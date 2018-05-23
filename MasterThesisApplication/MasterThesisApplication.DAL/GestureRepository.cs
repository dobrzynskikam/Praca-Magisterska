using System;
using MasterThesisApplication.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace MasterThesisApplication.DAL
{
    public class GestureRepository : IGestureRepository
    {
        static string _assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private readonly string _databasePath = Path.Combine(_assemblyPath.Replace("MasterThesisApplication\\bin\\Debug", ""), "GestureDatabase");
        private readonly string _gesturePath = Path.Combine(_assemblyPath, @"Gestures.xml");
        private readonly string _tempPath = Path.Combine(_assemblyPath, @"TempGestures.xml");
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

        public void AddNewGesture(Gesture gesture)
        {
            XDocument xmlDocument = XDocument.Load(_gesturePath);
            IEnumerable<XElement> gestures = xmlDocument.Element("Gestures").Elements("Gesture");

            //var temp = gestures.ElementAt(gestures.Count());
            var lastLabelNumber = int.Parse(gestures.Last(g => g != null).Attribute("Label").Value);
            //var maxLabel = int.Parse(gestures.ElementAt(gestures.Count()).Attribute("Label").Value);
            var gestureFolder = Path.Combine(_databasePath, gesture.GestureName);
            Directory.CreateDirectory(gestureFolder);
            var rootElement = xmlDocument.Element("Gestures");
            rootElement.Add(new XElement("Gesture", 
                new XAttribute("Name", gesture.GestureName),
                new XAttribute("Label", lastLabelNumber + 1),
                new XAttribute("BowNumber", gesture.BowNumber)
                    ));

            var gestureElement = rootElement.Elements("Gesture")
                .First(g => g.Attribute("Name").Value == gesture.GestureName);
            int index = 1;
            foreach (var feature in gesture.FeatureList)
            {
                var imageName = $"{gesture.GestureName}-train{index}.jpg";
                gestureElement.Add(new XElement("Feature", new XAttribute("ImageName", imageName)));
                File.Copy(feature.ImageName, Path.Combine(gestureFolder, imageName));
                index += 1;
            }
            xmlDocument.Save(_tempPath);

            //Move file
            
            //var last = gestures.Descendants("Gesture").First(g => (int)g.Attribute("Label") == 3);

            //xmlDocument.Element("Gestures").Elements("Gesture").ToList().Add(new XElement("Gesture", new XAttribute("Name", gestureName)));
            //xmlDocument.Save(_tempPath);
        }


        private void LoadGestures()
        {
            _gestures = new ObservableCollection<Gesture>();
            
            XDocument xmlDocument = XDocument.Load(_gesturePath);
            IEnumerable<XElement> gestures = xmlDocument.Element("Gestures").Elements("Gesture");

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
