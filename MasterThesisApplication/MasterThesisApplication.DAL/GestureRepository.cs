using System;
using MasterThesisApplication.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Xml.Linq;

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
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw new InvalidOperationException(), @"Gestures.xml");
            XDocument xmlDocument = XDocument.Load(path);
            IEnumerable<XElement> gestures = xmlDocument.Elements().Elements("Gesture");

            foreach (var gesture in gestures)
            {
                _features = new ObservableCollection<Feature>();
                var featureList = gesture.Elements("Feature");
                foreach (var feature in featureList)
                {
                    _features.Add(new Feature()
                    {
                        Vector = feature.Attribute("Vector")?.Value,
                        ImageName = feature.Attribute("ImageName")?.Value
                    });
                }
                _gestures.Add(new Gesture()
                {
                    GestureName = gesture.Attribute("Name")?.Value,
                    FeatureList = _features
                });
            }
        }
    }
}
