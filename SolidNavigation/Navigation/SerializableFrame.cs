using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace SolidNavigation.Navigation
{
    public class SerializableFrame : Frame
    {
        public PageStackEntry CurrentPageStackEntry { get; set; }

        public SerializableFrame()
        {
            this.Navigated += SerializableFrame_Navigated;
        }

        private void SerializableFrame_Navigated(object sender, NavigationEventArgs e)
        {
            CurrentPageStackEntry = new PageStackEntry(e.SourcePageType, e.Parameter, null);
        }

        public void Resume(Stream stream, Navigator navigator)
        {
            var deserializer = new DataContractJsonSerializer(typeof(FrameStack), new DataContractJsonSerializerSettings());
            var stack = (FrameStack)deserializer.ReadObject(stream);

            navigator.ToUrl(stack.Current);

            foreach (var item in stack.Back)
            {
                BackStack.Add(new PageStackEntry(typeof(MainPage), item, null));
            }

            foreach (var item in stack.Forward)
            {
                ForwardStack.Add(new PageStackEntry(typeof(MainPage), item, null));
            }
        }

        public Stream GetSuspensionStream()
        {
            var stack = new FrameStack
            {
                Back = (from pse in BackStack
                        select pse.Parameter + "").ToList(),
                Current = CurrentPageStackEntry.Parameter + "",
                Forward = (from pse in ForwardStack
                           select pse.Parameter + "").ToList(),
            };

            var ms = new MemoryStream();
            var serializer = new DataContractJsonSerializer(typeof(FrameStack), new DataContractJsonSerializerSettings());

            serializer.WriteObject(ms, stack);
            ms.Position = 0;
            return ms;
        }

        public class FrameStack
        {
            public List<string> Back { get; set; }
            public List<string> Forward { get; set; }
            public string Current { get; set; }
        }
    }
}
