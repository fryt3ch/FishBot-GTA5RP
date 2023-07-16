using System;
using System.Xml;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Xml.Schema;
using System.Text;

namespace FishBot
{
    public static class Config
    {
        private static XmlDocument xConfig = new XmlDocument();
        private static XmlDocument xResolutions = new XmlDocument();

        public static readonly string ConfigPath = "config.xml";

        public static string CaptchaServerIP = Properties.Resources.captcha_server.Substring(0, Properties.Resources.captcha_server.IndexOf(":"));
        public static int CaptchaServerPort = int.Parse(Properties.Resources.captcha_server.Substring(Properties.Resources.captcha_server.IndexOf(":") + 1));

        public static string Key = "";
        public static string CaptchaApiKey = "";
        public static string HWID = "";

        //menu_settings
        public static double Opacity;
        public static XmlNode UserKeyNode;

        public static int TimeRemain = 0;
        public static int CaptchaTimeRemain = 0;

        //bot_settings
        public static string GameResolution = "1920x1080";

        public static bool RealisticOn { get => bool.Parse(Nodes["realistic_on"].Attributes[0].Value); }
        public static bool SoundsOn { get => bool.Parse(Nodes["sounds_on"].Attributes[0].Value); }
        public static int ClickLatency { get => int.Parse(Nodes["click_latency"].Attributes[0].Value); }
        public static bool BlockMouse { get => bool.Parse(Nodes["block_mouse"].Attributes[0].Value); }
        public static bool MoodCheck { get => bool.Parse(Nodes["mood_check"].Attributes[0].Value); }
        public static bool HungerCheck { get => bool.Parse(Nodes["hunger_check"].Attributes[0].Value); }
        public static bool AutoEat { get => bool.Parse(Nodes["auto_eat"].Attributes[0].Value); }
        public static bool AutoSmoke { get => bool.Parse(Nodes["auto_smoke"].Attributes[0].Value); }
        public static bool ShiftBoat { get => bool.Parse(Nodes["boat_shift"].Attributes[0].Value); }
        public static bool DropFish { get => bool.Parse(Nodes["drop_fish"].Attributes[0].Value); }
        public static bool BypassCaptcha { get => bool.Parse(Nodes["bypass_captcha"].Attributes[0].Value); }
        public static bool SubtractBait { get => bool.Parse(Nodes["subtract_bait"].Attributes[0].Value); }
        public static bool FaultStop { get => bool.Parse(Nodes["fault_stop"].Attributes[0].Value); }
        public static bool TapRobot { get => bool.Parse(Nodes["tap_robot"].Attributes[0].Value); }
        public static bool VKNotificate { get => bool.Parse(Nodes["vk_notificate"].Attributes[0].Value); }
        public static bool OverweightBehaviour { get => bool.Parse(Nodes["overweight_behaviour"].Attributes[0].Value); }

        //colours
        public static Color HungerColor;
        public static Color MoodColor;
        public static Color CanFishColor;
        public static Color RodColor;
        public static Color BaitColor;
        public static Color ClickColor;
        public static Color WaitColor;
        public static Color SuccessColor;
        public static Color FaultColor;
        public static Color CaptchaColor;
        public static Color CigarettesColor;

        //rects
        public static Rectangle SmilesRect { get => Resolutions[GameResolution].SmilesRect; }
        public static Rectangle MouseRect { get => Resolutions[GameResolution].MouseRect; }
        public static Rectangle ResultRect { get => Resolutions[GameResolution].ResultRect; }
        public static Rectangle FishRect { get => Resolutions[GameResolution].FishRect; }
        public static Rectangle CaptchaTextRect { get => Resolutions[GameResolution].CaptchaTextRect; }
        public static Rectangle CaptchaRect { get => Resolutions[GameResolution].CaptchaRect; }
        public static Rectangle InventoryRect { get => Resolutions[GameResolution].InventoryRect; }
        public static Rectangle InvWeightRect { get => Resolutions[GameResolution].InvWeightRect; }
        public static Rectangle InventoryWBagRect { get => Resolutions[GameResolution].InventoryWBagRect; }
        public static Rectangle InvWBagWeightRect { get => Resolutions[GameResolution].InvWBagWeightRect; }
        public static Rectangle BagRect { get => Resolutions[GameResolution].BagRect; }
        public static Rectangle BoatInvRect { get => Resolutions[GameResolution].BoatInvRect; }

        //points
        public static Point DropMaxPoint { get => Resolutions[GameResolution].DropMaxPoint; }
        public static Point DropApplyPoint { get => Resolutions[GameResolution].DropApplyPoint; }
        public static Point SplitHalfPoint { get => Resolutions[GameResolution].SplitHalfPoint; }
        public static Point SplitMinPoint { get => Resolutions[GameResolution].SplitMinPoint; }
        public static Point CaptchaStartPoint { get => Resolutions[GameResolution].CaptchaStartPoint; }
        public static Point BoatClosePoint { get => Resolutions[GameResolution].BoatClosePoint; }
        public static Point CaptchaApplyPoint { get => Resolutions[GameResolution].CaptchaApplyPoint; }

        //other
        public static int InvBoxSide = 0;
        public static int BinarizationCaptcha = 0;
        public static int BinarizationItems = 0;
        public static int BinarizationWeight = 0;
        public static int BinarizationFish = 0;

        public class Phrase
        {
            public string Text;
            public Color Color;

            public Phrase(string Text, Color Color)
            {
                this.Text = Text;
                this.Color = Color;
            }

        }

        public static Dictionary<string, Phrase> Phrases = new Dictionary<string, Phrase>();

        public static List<string> FoodList = new List<string>();
        public static List<Fish> FishList = new List<Fish>();
        public static List<string> FishToDropList = new List<string>();

        public static Dictionary<string, XmlNode> Nodes = new Dictionary<string, XmlNode>();

        public static Dictionary<string, Resolution> Resolutions = new Dictionary<string, Resolution>();

        public static StreamWriter sw;
        public static FileStream fs;

        public class Resolution
        {
            //rects
            public Rectangle SmilesRect;
            public Rectangle MouseRect;
            public Rectangle ResultRect;
            public Rectangle FishRect;
            public Rectangle CaptchaTextRect;
            public Rectangle CaptchaRect;
            public Rectangle InventoryRect;
            public Rectangle InvWeightRect;
            public Rectangle InventoryWBagRect;
            public Rectangle InvWBagWeightRect;
            public Rectangle BagRect;
            public Rectangle BoatInvRect;

            //points
            public Point DropMaxPoint;
            public Point DropApplyPoint;
            public Point SplitMaxPoint;
            public Point SplitHalfPoint;
            public Point SplitMinPoint;
            public Point CaptchaStartPoint;
            public Point BoatClosePoint;
            public Point CaptchaApplyPoint;
        }

        private static void ValidateXml()
        {
            XmlSchemaInference infer = new XmlSchemaInference();
            XmlSchemaSet schemaSet =
              infer.InferSchema(new XmlTextReader(new MemoryStream(Encoding.UTF8.GetBytes(Properties.Resources.config))));

            xConfig.Load(ConfigPath);

            xConfig.Schemas = schemaSet;
            xConfig.Validate((sender, e) =>
            {
                Debug.WriteLine("Invalid check by schema");
                throw new Exception(e.Message);
            });
        }

        public static void LoadConfig(bool validate = true)
        {
            Nodes.Clear();
            FoodList.Clear();
            Phrases.Clear();

            if (validate)
                ValidateXml();

            XmlElement xRoot = xConfig.DocumentElement;

            foreach (XmlNode xNode in xRoot)
            {
                if (xNode.Name == "menu_settings")
                {
                    foreach (XmlNode msNode in xNode)
                    {
                        Nodes.Add(msNode.Name, msNode);

                        if (msNode.Name == "opacity")
                            //Opacity = int.Parse(msNode.Attributes.GetNamedItem("value").Value) / 100f;
                            Opacity = 1f;
                        else if (msNode.Name == "user_key")
                            UserKeyNode = msNode;
                        else if (msNode.Name == "captcha_api_key")
                            CaptchaApiKey = msNode.Attributes.GetNamedItem("value").Value;
                    }

                    continue;
                }

                if (xNode.Name == "bot_settings")
                {
                    foreach (XmlNode bsNode in xNode)
                        Nodes.Add(bsNode.Name, bsNode);

                    continue;
                }

                if (xNode.Name == "food")
                {
                    if (xNode.Attributes[0].Name == "list")
                    {
                        FoodList = xNode.Attributes[0].Value.ToLower().Split(' ').ToList();

                        Inventory.SupportedItemNames.AddRange(FoodList);
                    }

                    continue;
                }

                if (xNode.Name == "fish_to_drop")
                {
                    if (xNode.Attributes[0].Name == "list")
                        FishToDropList = xNode.Attributes[0].Value.ToLower().Split(' ').ToList();

                    continue;
                }

                if (xNode.Name == "phrases")
                {
                    foreach (XmlNode pNode in xNode)
                    {
                        Color col = Color.FromArgb(int.Parse(pNode.Attributes[0].Value), int.Parse(pNode.Attributes[1].Value), int.Parse(pNode.Attributes[2].Value));

                        foreach (XmlNode fNode in pNode)
                            Phrases.Add(fNode.Name, new Phrase(fNode.Attributes[0].Value, col));
                    }

                    continue;
                }

                if (xNode.Name == "prices")
                {
                    foreach (XmlNode pNode in xNode)
                    {
                        Nodes.Add(pNode.Name, pNode);

                        if (pNode.Name == "min")
                            foreach (XmlNode fish in pNode.Attributes)
                            {
                                Fish item = FishList.Find(x => x.Name == fish.Name);

                                if (item != null)
                                    item.MinCost = int.Parse(fish.Value);
                                else
                                {
                                    FishList.Add(new Fish(fish.Name) { MinCost = int.Parse(fish.Value) });

                                    Inventory.SupportedItemNames.Add(fish.Name);
                                }
                            }

                        if (pNode.Name == "max")
                            foreach (XmlNode fish in pNode.Attributes)
                            {
                                Fish item = FishList.Find(x => x.Name == fish.Name);

                                if (item != null)
                                    item.MaxCost = int.Parse(fish.Value);
                                else
                                {
                                    FishList.Add(new Fish(fish.Name) { MaxCost = int.Parse(fish.Value) });

                                    Inventory.SupportedItemNames.Add(fish.Name);
                                }
                            }
                    }

                    continue;
                }

                if (xNode.Name == "weights")
                {
                    foreach (XmlNode fish in xNode.Attributes)
                    {
                        Fish item = FishList.Find(x => x.Name == fish.Name);

                        if (item != null)
                            item.Weight = double.Parse(fish.Value, System.Globalization.NumberStyles.Any);
                        else
                        {
                            FishList.Add(new Fish(fish.Name) { Weight = double.Parse(fish.Value, System.Globalization.NumberStyles.Any) });

                            Inventory.SupportedItemNames.Add(fish.Name);
                        }
                    }

                    continue;
                }

                    if (xNode.Name == "colours")
                {
                    foreach (XmlNode cNode in xNode)
                    {
                        Color temp = Color.FromArgb(
                            int.Parse(cNode.Attributes.GetNamedItem("R").Value),
                            int.Parse(cNode.Attributes.GetNamedItem("G").Value),
                            int.Parse(cNode.Attributes.GetNamedItem("B").Value));

                        if (cNode.Name == "hunger")
                            HungerColor = temp;
                        else if (cNode.Name == "mood")
                            MoodColor = temp;
                        else if (cNode.Name == "can_fish")
                            CanFishColor = temp;
                        else if (cNode.Name == "rod")
                            RodColor = temp;
                        else if (cNode.Name == "bait")
                            BaitColor = temp;
                        else if (cNode.Name == "click")
                            ClickColor = temp;
                        else if (cNode.Name == "wait")
                            WaitColor = temp;
                        else if (cNode.Name == "success")
                            SuccessColor = temp;
                        else if (cNode.Name == "fault")
                            FaultColor = temp;
                        else if (cNode.Name == "captcha")
                            CaptchaColor = temp;
                        else if (cNode.Name == "cigarettes")
                            CigarettesColor = temp;
                    }

                    continue;
                }
            }
        }

        public static void LoadResolutions(string xml)
        {
            Resolutions.Clear();

            //ValidateXml(FolderPath + ResolutionsPath, false);
            xResolutions.LoadXml(xml);

            XmlElement xRoot = xResolutions.DocumentElement;

            foreach (XmlNode xNode in xRoot)
            {
                if (xNode.Name == "binarization")
                {
                    BinarizationCaptcha = int.Parse(xNode.Attributes.GetNamedItem("captcha").Value);
                    BinarizationItems = int.Parse(xNode.Attributes.GetNamedItem("items").Value);
                    BinarizationWeight = int.Parse(xNode.Attributes.GetNamedItem("weight").Value);
                    BinarizationFish = int.Parse(xNode.Attributes.GetNamedItem("fish").Value);
                }
                else if (xNode.Name == "invbox_side")
                    InvBoxSide = int.Parse(xNode.Attributes.GetNamedItem("value").Value);
                else if (xNode.Name == "resolutions")
                {
                    foreach (XmlNode rNode in xNode)
                    {
                        string curResolution = rNode.Attributes.GetNamedItem("value").Value;

                        Resolutions.Add(curResolution, new Resolution());

                        foreach (XmlNode rectNode in rNode)
                            if (rectNode.Name == "rect")
                            {
                                Rectangle temp = new Rectangle(
                                    int.Parse(rectNode.Attributes.GetNamedItem("X").Value),
                                    int.Parse(rectNode.Attributes.GetNamedItem("Y").Value),
                                    int.Parse(rectNode.Attributes.GetNamedItem("width").Value),
                                    int.Parse(rectNode.Attributes.GetNamedItem("height").Value));

                                if (rectNode.Attributes.GetNamedItem("value").Value == "smiles")
                                    Resolutions[curResolution].SmilesRect = temp;
                                else if (rectNode.Attributes.GetNamedItem("value").Value == "mouse")
                                    Resolutions[curResolution].MouseRect = temp;
                                else if (rectNode.Attributes.GetNamedItem("value").Value == "result")
                                    Resolutions[curResolution].ResultRect = temp;
                                else if (rectNode.Attributes.GetNamedItem("value").Value == "fish")
                                    Resolutions[curResolution].FishRect = temp;
                                else if (rectNode.Attributes.GetNamedItem("value").Value == "captcha_text")
                                    Resolutions[curResolution].CaptchaTextRect = temp;
                                else if (rectNode.Attributes.GetNamedItem("value").Value == "captcha")
                                    Resolutions[curResolution].CaptchaRect = temp;
                                else if (rectNode.Attributes.GetNamedItem("value").Value == "inventory")
                                    Resolutions[curResolution].InventoryRect = temp;
                                else if (rectNode.Attributes.GetNamedItem("value").Value == "inv_weight")
                                    Resolutions[curResolution].InvWeightRect = temp;
                                else if (rectNode.Attributes.GetNamedItem("value").Value == "inventory_wbag")
                                    Resolutions[curResolution].InventoryWBagRect = temp;
                                else if (rectNode.Attributes.GetNamedItem("value").Value == "inv_wbag_weight")
                                    Resolutions[curResolution].InvWBagWeightRect = temp;
                                else if (rectNode.Attributes.GetNamedItem("value").Value == "bag")
                                    Resolutions[curResolution].BagRect = temp;
                                else if (rectNode.Attributes.GetNamedItem("value").Value == "boat_inv")
                                    Resolutions[curResolution].BoatInvRect = temp;
                            }
                            else if (rectNode.Name == "point")
                            {
                                Point temp = new Point(
                                    int.Parse(rectNode.Attributes.GetNamedItem("X").Value),
                                    int.Parse(rectNode.Attributes.GetNamedItem("Y").Value)
                                    );

                                if (rectNode.Attributes.GetNamedItem("value").Value == "drop_max")
                                    Resolutions[curResolution].DropMaxPoint = temp;
                                else if (rectNode.Attributes.GetNamedItem("value").Value == "drop_apply")
                                    Resolutions[curResolution].DropApplyPoint = temp;
                                else if (rectNode.Attributes.GetNamedItem("value").Value == "split_max")
                                    Resolutions[curResolution].SplitMaxPoint = temp;
                                else if (rectNode.Attributes.GetNamedItem("value").Value == "split_half")
                                    Resolutions[curResolution].SplitHalfPoint = temp;
                                else if (rectNode.Attributes.GetNamedItem("value").Value == "split_min")
                                    Resolutions[curResolution].SplitMinPoint = temp;
                                else if (rectNode.Attributes.GetNamedItem("value").Value == "captcha_start")
                                    Resolutions[curResolution].CaptchaStartPoint = temp;
                                else if (rectNode.Attributes.GetNamedItem("value").Value == "boat_close")
                                    Resolutions[curResolution].BoatClosePoint = temp;
                                else if (rectNode.Attributes.GetNamedItem("value").Value == "captcha_apply")
                                    Resolutions[curResolution].CaptchaApplyPoint = temp;
                            }
                    }
                }
            }
        }

        public static void LoadDefaultConfig()
        {
            xConfig.LoadXml(Properties.Resources.config);
            LoadConfig(false);

            SaveConfig();
        }

        public static void SaveConfig()
        {
            fs.SetLength(0);
            xConfig.Save(sw);
        }

        public static string GetNodeValue(string parameterName) => Nodes[parameterName].Attributes[0].Value;

        public static void UpdateParameter(string parameterName, string value)
        {
            if (value == null || !Nodes.ContainsKey(parameterName))
                return;

            if (Nodes[parameterName].Attributes[0].Value != value.ToString())
            {
                Nodes[parameterName].Attributes[0].Value = value.ToString();
                SaveConfig();
            }
        }
    }
}