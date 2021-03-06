using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Drawing.Printing;

namespace scriptASS
{
    class ASSDataGridViewTextBoxCell : DataGridViewTextBoxCell
    {

        
        ASSTextBoxRegEx textbox;
        public ASSDataGridViewTextBoxCell()
            : base()
        {
            /*
            textbox = new ASSTextBoxRegEx();
            textbox.Font = this.Style.Font;
            textbox.Size = this.Size;                        
             */
        }
        

        private static bool DelTags = false;
        private static bool SubsTags = false;
        private static string Token = " ƒ ";

        //string renderCell = Application.StartupPath + "perrySub.Render";

        protected override void Paint(System.Drawing.Graphics graphics, System.Drawing.Rectangle clipBounds, System.Drawing.Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
        {


            string valor = (string)formattedValue;

            if (ASSDataGridViewTextBoxCell.DelTags) valor = lineaASS.cleanText(formattedValue.ToString());
            if (ASSDataGridViewTextBoxCell.SubsTags)
            {
                Regex r = new Regex(@"\{\S+?\}");
                valor = r.Replace(valor, Token);
            }
            /*
            textbox.Text = valor;
            textbox.ForceRefresh();
            */
            base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, value, valor, errorText, cellStyle, advancedBorderStyle, paintParts);
            /*
            Size largeSize = this.Size;
            Bitmap bmp = new Bitmap(largeSize.Width, largeSize.Height);

            Graphics g = Graphics.FromImage(bmp);
            g.Clear(this.Style.BackColor);

            PrinterSettings pSet = new PrinterSettings();

            PageSettings pageSettings = new PageSettings();
            pageSettings.Color = true;
            pageSettings.Landscape = false;
            pageSettings.Margins = new Margins(0, 0, 0, 0);
            pageSettings.PaperSize = new PaperSize("Letter", cellBounds.Width,cellBounds.Height);
            //pageSettings.PaperSize.RawKind = (int)PaperKind.Letter;

            PrintPageEventArgs args = new PrintPageEventArgs(g,clipBounds,
                                                                cellBounds,
                                                                pageSettings);

            textbox.Print(0, textbox.TextLength, args);

            g.Dispose();

            //bmp.Save("file2.bmp", System.Drawing.Imaging.ImageFormat.Png);

            graphics.DrawImage(bmp,cellBounds);
            */
        }

        public static void SetDeleteTags(bool t)
        {
            ASSDataGridViewTextBoxCell.DelTags = t;
        }

        public static void SetSubstituteTags(bool t)
        {
            ASSDataGridViewTextBoxCell.SubsTags = t;
        }

    }
}
