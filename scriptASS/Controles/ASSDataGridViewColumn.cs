using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace scriptASS
{
    public class ASSDataGridViewTextBoxColumn : DataGridViewTextBoxColumn
        {
        public ASSDataGridViewTextBoxColumn()
            {
                this.CellTemplate = new ASSDataGridViewTextBoxCell();
            }
        }
}
