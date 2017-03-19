using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ABC.Navigation
{
    public class UrlSerializer
    {
        private IEnumerable<Route> _routes;
        private string _scheme;

        public UrlSerializer(string scheme, IEnumerable<Route> routes)
        {
            _routes = routes;
            _scheme = scheme;
        }

        private Route FindRouteOrDefault(INavigationTarget target)
        {
            var route = _routes.FirstOrDefault(x => x.TargetType == target.GetType());
            if (route == null)
            {
                return _routes.Last(); // default route (home)
            }
            return route;
        }

        public Route FindRouteOrDefault(string url)
        {
            var route = _routes.FirstOrDefault(r => r.IsMatch(url));
            if (route == null)
            {
                return _routes.Last(); // default route (home)
            }
            return route;
        }

        public string CreateUrl(INavigationTarget target)
        {
            var route = FindRouteOrDefault(target);
            return CreateUrl(route, target);
        }

        private string CreateUrl(Route route, INavigationTarget uriInfo)
        {
            var url = _scheme + route.UrlPattern;
            var props = GetPropertiesHierarchical(uriInfo.GetType());
            var routeVars = route.CreateSegments().Where(x => x.IsVariable).Select(x => x.Segment.ToLowerInvariant());
            foreach (var prop in props)
            {
                if (routeVars.Contains(prop.Name.ToLowerInvariant()))
                {
                    var value = Uri.EscapeDataString(prop.GetValue(uriInfo)+"");
                    url = url.Replace("{" + prop.Name.ToLowerInvariant() + "}", value);
                }
                //else if (prop.PropertyType == typeof(string) || prop.PropertyType.GetTypeInfo().IsValueType)
                //{
                //    // querystring parameters ?id=x&id2=y
                //    var value = Uri.EscapeDataString((string)prop.GetValue(uriInfo));
                //    // building querystring parameters. If a ? already occurs, add a parameter with &, else add the first parameter with ?
                //    if (url.Contains("?"))
                //    {
                //        url += "&";
                //    }
                //    else
                //    {
                //        url += "?";
                //    }
                //    url += $"{prop.Name.ToLowerInvariant()}={value}";
                //}
            }
            return url;
        }

        public INavigationTarget CreateNavigationTarget(string url)
        {
            if (!url.StartsWith(_scheme))
            {
                url = $"{_scheme}{url}";
            }

            var uriInfo = new UriInfo(url);
            var route = FindRouteOrDefault(url);

            var parameters = ExtractParameters(route, uriInfo);
            var ctor = GetConstructor(route.TargetType, parameters);

            if (ctor == null)
            {
                throw new Exception($"Can't find constructor in {route.TargetType} to match {parameters.Count} parameters {String.Join(", ", parameters)}");
            }

            var ctorparams = ctor.GetParameters();

            var parameterValues = new List<object>();
            foreach (var item in parameters)
            {
                var param = ctorparams.FirstOrDefault(x => x.Name.ToLowerInvariant() == item.Key.ToLowerInvariant());
                if (param != null)
                {
                    var filledparam = ChangeType(item.Value, param.ParameterType);
                    parameterValues.Add(filledparam);
                }
            }
            var target = CreateTarget(ctor, parameterValues, url);
            return target;
        }

        private ConstructorInfo GetConstructor(Type type, Dictionary<string, string> parameters)
        {
            var parameterNames = parameters.Select(p => p.Key);
            var constructors = type.GetTypeInfo().DeclaredConstructors.OrderByDescending(c => c.GetParameters().Length);
            return constructors.FirstOrDefault(c => IsParameterMatch(c.GetParameters(), parameterNames));
        }

        private INavigationTarget CreateTarget(ConstructorInfo ctor, List<object> parameters, string url)
        {
            try
            {
                return ctor.Invoke(parameters.ToArray()) as INavigationTarget;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Failed to create target for URI '{url}'", ex);
            }
        }

        private bool IsParameterMatch(IEnumerable<ParameterInfo> parameters, IEnumerable<string> parameterNames) => parameters.All(p => parameterNames.Contains(p.Name.ToLowerInvariant()));

        private object ChangeType(string value, Type type) => Convert.ChangeType(Uri.UnescapeDataString(value), type);

        private Dictionary<string, string> ExtractParameters(Route route, UriInfo uriInfo)
        {
            var parameters = new Dictionary<string, string>();
            var segments = route.CreateSegments();

            for (var i = 0; i < segments.Count; i++)
            {
                if (segments[i].IsVariable && uriInfo.Segments.Length > i)
                {
                    parameters.Add(segments[i].Segment.ToLowerInvariant(), uriInfo.Segments[i]);
                }
            }

            foreach (var qs in uriInfo.QueryString)
            {
                parameters.Add(qs.Key.ToLowerInvariant(), qs.Value);
            }

            return parameters;
        }

        private IEnumerable<PropertyInfo> GetPropertiesHierarchical(Type type)
        {
            if (type.Equals(typeof(object)))
            {
                return type.GetTypeInfo().DeclaredProperties;
            }

            return type.GetTypeInfo().DeclaredProperties.Concat(GetPropertiesHierarchical(type.GetTypeInfo().BaseType));
        }
    }
}
