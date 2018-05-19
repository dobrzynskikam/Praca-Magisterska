using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MasterThesisApplication.Model;

namespace MasterThesisApplication.DAL
{
    public class GestureRepository : IGestureRepository
    {
        private ObservableCollection<Gesture> _gestures;
        private ObservableCollection<Feature> _features;
        public ObservableCollection<Gesture> GetGestures()
        {
            if (_gestures == null)
            {
                LoadGestures();
            }

            return _gestures;
        }

        private void LoadGestures()
        {
            _gestures = new ObservableCollection<Gesture>();
            

            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Gestures.xml");
            XDocument xmlDocument = XDocument.Load(path);
            IEnumerable<XElement> gestures = xmlDocument.Elements().Elements("Gesture");
            var gestureNames = gestures.Select(g => g.Attribute("Name").Value);
            var aGesture = gestures.Where(g => g.Attribute("Name").Value == "A");
            IEnumerable<XElement> aFeatures = aGesture.Elements("Feature");
            var aVectors = aFeatures.Select(f => f.Attribute("Vector").Value);

            foreach (var gesture in gestures)
            {
                _features = new ObservableCollection<Feature>();
                var featureList = gesture.Elements("Feature");
                foreach (var feature in featureList)
                {
                    _features.Add(new Feature()
                    {
                        Vector = feature.Attribute("Vector").Value
                    });
                }
                _gestures.Add(new Gesture()
                {
                    GestureName = gesture.Attribute("Name").Value,
                    FeatureList = _features
                });
            }
        }
    }
}
