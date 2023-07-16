using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tesseract;

namespace FishBot
{
    class Inventory
    {
        public Rectangle Rectangle { get; }

        public static List<string> SupportedItemNames = new List<string>
        {
            "удочка", "приманка", "сигареты", "вейп" , "empty", "other"
            // Fish and Food will be added after it was loaded from config
        };

        public Dictionary<string, List<Point>> Items = new Dictionary<string, List<Point>>();

        public Inventory(Rectangle Rectangle) => this.Rectangle = Rectangle;

        public void UpdateItems(Bitmap Bm)
        {
            Items.Clear();

            foreach (var item in SupportedItemNames)
                Items.Add(item, new List<Point>());

            for (int i = 0; i < Rectangle.Height / Config.InvBoxSide + 1; i++)
                for (int j = 0; j < Rectangle.Width / Config.InvBoxSide + 1; j++)
                {
                    Point pnt = new Point(Rectangle.X + Config.InvBoxSide * j + Config.InvBoxSide / 2, Rectangle.Y + Config.InvBoxSide * i + Config.InvBoxSide / 2);

                    Items[GetBoxItemName(Bm, pnt)].Add(pnt);
                }
        }

        public string GetBoxItemName(Bitmap Bm, Point CenterPoint)
        {
            int Binarization = Config.BinarizationItems;
            string res = "empty";

            bool PotentiallyOtherItem = false;

            Bitmap box = Bm.CropAtRect(new Rectangle(CenterPoint.X - Rectangle.X - Config.InvBoxSide / 2, CenterPoint.Y - Rectangle.Y - Config.InvBoxSide / 2, Config.InvBoxSide, Config.InvBoxSide));

            do
            {
                if (Binarization <= 50)
                {
                    if (PotentiallyOtherItem)
                        res = "other";
                    else
                        res = "empty";

                    break;
                }

                res = ((Bitmap)box.Clone()).BinarizeImage(Binarization -= 10).GetText(Capture.TesseractAPI).ToLower();

                if (res.Contains("принанка") || res.Contains("приванка"))
                    res = "приманка";
                else if (res.Contains("осетр"))
                    res = "осётр";

                if (res.Contains("веип"))
                    res = "вейп";

                bool OK = false;

                foreach (var x in SupportedItemNames)
                    if (res.Contains(x))
                    {
                        OK = true;

                        res = x;

                        break;
                    }

                if (OK)
                    break;

                if (res.Length > 0)
                    PotentiallyOtherItem = true;
            }
            while (true);

            return res;
        }
    }

    static class Extensions
    {
        public static bool Shift(this Inventory SourceInv, Inventory TargetInv, List<string> ItemNamesToShift, IntPtr GameWindowHandle)
        {
            Bitmap Bitmap = new Bitmap(1, 1);

            SourceInv.UpdateItems(Bitmap.Update(GameWindowHandle, SourceInv.Rectangle));

            if (SourceInv.Items.Where(x => (x.Value.Count > 0 && ItemNamesToShift.Contains(x.Key))).Count() == 0)
                return false;

            TargetInv.UpdateItems(Bitmap.Update(GameWindowHandle, TargetInv.Rectangle));

            foreach (var ItemName in ItemNamesToShift)
            {
                if (SourceInv.Items[ItemName].Count != 0 && TargetInv.Items[ItemName].Count != 0)
                {
                    for (int i = 0; i < SourceInv.Items[ItemName].Count; i++)
                        for (int j = 0; j < TargetInv.Items[ItemName].Count; j++)
                        {
                            KeysManager.DragMouse(GameWindowHandle, SourceInv.Items[ItemName][i], TargetInv.Items[ItemName][j]);

                            Point SourceItemPoint = SourceInv.Items[ItemName][i];

                            if (SourceInv.GetBoxItemName(Bitmap.Update(GameWindowHandle, SourceInv.Rectangle), SourceItemPoint) == "empty")
                            {
                                SourceInv.Items[ItemName].RemoveAt(i--);
                                SourceInv.Items["empty"].Add(SourceItemPoint);

                                break;
                            }
                        }
                }

                if (SourceInv.Items[ItemName].Count != 0)
                {
                    for (int i = 0; i < SourceInv.Items[ItemName].Count; i++)
                    {
                        if (TargetInv.Items["empty"].Count > 0)
                        {
                            Point EmptySlotPoint = TargetInv.Items["empty"][0];

                            KeysManager.DragMouse(GameWindowHandle, SourceInv.Items[ItemName][i], EmptySlotPoint);

                            if (TargetInv.GetBoxItemName(Bitmap.Update(GameWindowHandle, TargetInv.Rectangle), EmptySlotPoint) != "empty")
                            {
                                TargetInv.Items[ItemName].Add(EmptySlotPoint);
                                TargetInv.Items["empty"].RemoveAt(0);

                                SourceInv.Items["empty"].Add(SourceInv.Items[ItemName][i]);
                                SourceInv.Items[ItemName].RemoveAt(i--);

                                continue;
                            }
                            else if (i >= SourceInv.Items[ItemName].Count - 1)
                                break;
                        }
                    }
                }
            }

            return true;
        }

        public static (double Weight, double MaxWeight) GetWeight(this Bitmap bm, int binarizationLevel = 0)
        {
            IFormatProvider formatter = new NumberFormatInfo { NumberDecimalSeparator = "," };

            Bitmap bin = (Bitmap)bm.Clone();
            string res = string.Join("", bin.BinarizeImage(binarizationLevel).GetText(Capture.TesseractNumAPI)).Replace('.', ',');

            if (binarizationLevel <= 50)
                return (-1, -1);
            else if (res.Length <= 0 || !res.Contains(",") || !res.Contains("/"))
                return GetWeight(bm, binarizationLevel - 5);

            (double weight, double maxWeight) weight = (9.7, 10);

            int slashInd = res.IndexOf('/');
            int dotInd = res.IndexOf(',');

            if (slashInd == -1 && dotInd != -1)
            {
                double.TryParse(res.Substring(0, dotInd + 3), NumberStyles.Any, formatter, out weight.weight);

                string maxWeightStr = res.Substring(dotInd + 3, res.Length - dotInd - 3);

                double.TryParse(maxWeightStr, NumberStyles.Any, formatter, out weight.maxWeight);
            }
            else if (slashInd != -1)
            {
                double.TryParse(res.Substring(0, dotInd + 3), NumberStyles.Any, formatter, out weight.weight);
                double.TryParse(res.Substring(slashInd + 1, res.Length - slashInd - 1), NumberStyles.Any, formatter, out weight.maxWeight);
            }

            if (weight.maxWeight < 10)
                weight.maxWeight = 10;

/*            string str = weight.weight.ToString();

            if (int.TryParse(str, out _) && weight.weight > 10)
                weight.weight = double.Parse(str.Insert(str.Length - 2, ","));*/

            return weight;
        }
    }
}
