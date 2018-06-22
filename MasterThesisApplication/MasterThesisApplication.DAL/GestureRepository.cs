using System;
using System.Collections.Generic;
using MasterThesisApplication.Model;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Accord;

namespace MasterThesisApplication.DAL
{
    public class GestureRepository : IGestureRepository
    {
        private static readonly string AssemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static readonly string DatabasePath = Path.Combine(AssemblyPath.Replace("MasterThesisApplication\\bin\\Debug", ""), "GestureDatabase");
        private readonly string _gesturePath = Path.Combine(DatabasePath, @"Gestures.xml");
        private ObservableCollection<Gesture> _gestures;
        private ObservableCollection<Feature> _features;

        public ObservableCollection<Gesture> GetGestures()
        {
            LoadGestures();
            return _gestures;
        }

        public Dictionary<string, Tuple<double[], Bitmap>> GetImages()
        {
            var bitmapDictionary = new Dictionary<string, Tuple<double[], Bitmap>>();
            //var gestureCollection = GetGestures();
            //foreach (var gesture in gestureCollection)
            //{
            //    {
            //        foreach (var feature in gesture.FeatureList)
            //        {
            //            var imageFullName = Path.Combine(DatabasePath, gesture.GestureName, feature.ImageName);
            //            var vec = feature.Vector.Split(' ').Select(x => double.Parse(x)).ToArray();
            //            var bitMap = (Bitmap) Image.FromFile(imageFullName);

            //            bitmapDictionary.Add(feature.ImageName, new Tuple<double[], Bitmap>(vec, bitMap));
            //        }
            //    }
            //}

            return bitmapDictionary;
        }

        public void AddNewGesture(Gesture gesture)
        {
            ////Edit this function to work in gesture object, not in xml file!!!
            //var existingGestures = GetGestures();

            ////check if gestures database has any gesture
            //if (existingGestures.Count == 0)
            //{
            //    gesture.Label = 0;
            //    existingGestures.Add(gesture);
            //}

            XDocument xmlDocument = XDocument.Load(_gesturePath);
            var gestures = xmlDocument.Element("Gestures")?.Elements("Gesture");

            if (gestures == null) return;
            var rootElement = xmlDocument.Element("Gestures");
            int newLabelNumber = 0;

            //check if gestures database has any gesture
            if (rootElement.FirstNode != null)
            {
                newLabelNumber = int.Parse(gestures.Last(g => g != null).Attribute("Label")?.Value) + 1;
            }
            
            
            int imageIndex;
            string gestureFolder = Path.Combine(DatabasePath, gesture.GestureName);

            if (gestures.Any(g => g.Attribute("Name")?.Value == gesture.GestureName))
            {
                var gestureEl = rootElement?.Elements("Gesture")
                    .First(g => g.Attribute("Name")?.Value == gesture.GestureName);

                var lastImageFullName = gestureEl?.Elements("Feature").Last(e => e != null).Attribute("ImageName")?.Value;
                var lastImageName = Path.GetFileNameWithoutExtension(lastImageFullName);
                imageIndex = int.Parse(lastImageName?.Replace($"{gesture.GestureName}-train", "")) + 1;
            }
            else
            {
                //Add Folder for new Gesture
                Directory.CreateDirectory(gestureFolder);

                //Add new Gesture child to xml structure
                rootElement?.Add(new XElement("Gesture",
                    new XAttribute("Name", gesture.GestureName),
                    new XAttribute("Label", newLabelNumber)));

                //This is new gesture, so we start iterate images from 1
                imageIndex = 1;
            }

            var gestureElement = rootElement?.Elements("Gesture")
                .First(g => g.Attribute("Name")?.Value == gesture.GestureName);


            foreach (var feature in gesture.FeatureList)
            {
                var imageName = $"{gesture.GestureName}-train{imageIndex}.jpg";
                gestureElement?.Add(new XElement("Feature", new XAttribute("ImageName", imageName)));
                File.Copy(feature.ImageName, Path.Combine(gestureFolder, imageName));
                imageIndex += 1;
            }
            xmlDocument.Save(_gesturePath);
        }


        private void LoadGestures()
        {
            _gestures = new ObservableCollection<Gesture>();
            
            XDocument xmlDocument = XDocument.Load(_gesturePath);
            var gestures = xmlDocument.Element("Gestures")?.Elements("Gesture");

            if (gestures == null) return;
            foreach (var gesture in gestures)
            {
                _features = new ObservableCollection<Feature>();
                var featureList = gesture.Elements("Feature");
                foreach (var feature in featureList)
                {
                    _features.Add(new Feature()
                    {
                        Vector = feature.Attribute("Vector")?.Value.Split(' ').Select(c=>double.Parse(c)).ToArray(),
                        ImageName = feature.Attribute("ImageName")?.Value
                    });
                }

                _gestures.Add(new Gesture()
                {
                    GestureName = gesture.Attribute("Name")?.Value,
                    Label = int.Parse(gesture.Attribute("Label")?.Value),
                    FeatureList = _features
                });
            }
        }

        public void SaveGestures(ObservableCollection<Gesture> gestures)
        {
            //Load file
            XDocument xmlDocument = XDocument.Load(_gesturePath);

            //Remove all root descendants
            if (gestures == null) return;
            var rootElement = xmlDocument.Element("Gestures");
            rootElement.RemoveAll();

            foreach (var gesture in gestures)
            {
                //Add new gesture
                rootElement?.Add(new XElement("Gesture",
                    new XAttribute("Name", gesture.GestureName),
                    new XAttribute("Label", gesture.Label)));

                var gestureElement = rootElement?.Elements("Gesture")
                    .First(g => g.Attribute("Name")?.Value == gesture.GestureName);

                foreach (var feature in gesture.FeatureList)
                {
                    gestureElement?.Add(new XElement("Feature", 
                        new XAttribute("ImageName", feature.ImageName),
                        new XAttribute("Vector", feature.Vector)
                        ));
                }
            }
            xmlDocument.Save(_gesturePath);
        }
    }
}
