using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Drawing;

namespace scriptASS
{
    partial class mainW
    {

        ArrayList toFind = new ArrayList();

        public ArrayList TrimToSelected(ArrayList integerArray)
        {
            ArrayList temp = new ArrayList();
            foreach (int s in integerArray)
            {
                if (gridASS.SelectedRows.Contains(gridASS.Rows[s]))
                    temp.Add(s);
            }

            return temp;

        }

        public void doFind(ArrayList found, bool isSelected)
        {
            if (isSelected) // nos cargamos todo lo que no este en el rango
                found = TrimToSelected(found); //temp;

            if (found.Count == 0)
            {
                errorMsg("No se han encontrado coincidencias.");
                return;
            }

            clearSelectedRows();

            int firstidx = (int)found[0];
            gridASS.Rows[firstidx].Selected = true;

            moveViewRows(firstidx);

            toFind = found;
            updateMenuEnables();
        }

        public void doFindNext()
        {
            int pos_actual = gridASS.SelectedRows[0].Index;
            int pos_inArray = -1;

            for (int i = 0; i < toFind.Count; i++)
            {
                int memb = (int)toFind[i];
                if (pos_actual == memb)
                    pos_inArray = i;
            }

            if (pos_inArray == -1) return;

            if (pos_inArray == toFind.Count - 1) // reset si estamos al final
                pos_inArray = -1;

            pos_inArray++;
            int pos_nueva = (int)toFind[pos_inArray];
            clearSelectedRows();

            gridASS.Rows[pos_nueva].Selected = true;
            moveViewRows(pos_nueva);                        
        }

        public void doShift(String shifter, ShiftStyle style)
        {

            UndoRedo.AddUndo(script, "Shift times"); //UndoStack.Push(PrepareForPush(al));

            IsModified = true;

            bool isSum = (shifter.StartsWith("+")) ? true : false;
            String toShift = shifter.Substring(1);

            if (style == ShiftStyle.Selected)
            {
                for (int i = 0; i < gridASS.SelectedRows.Count; i++)
                {
                    lineaASS lass = (lineaASS)al[gridASS.SelectedRows[i].Index];
                    if (isSum)
                    {
                        lass.t_inicial.sumaTiempo(toShift);
                        lass.t_final.sumaTiempo(toShift);
                    }
                    else
                    {
                        lass.t_inicial.restaTiempo(toShift);
                        lass.t_final.restaTiempo(toShift);
                    }
                }
            }
            else
            {
                for (int i = 0; i < gridASS.Rows.Count; i++)
                {
                    lineaASS lass = (lineaASS)al[i];

                    if ((style == ShiftStyle.Normal) ||
                        (style == ShiftStyle.AfterSelected && i >= gridASS.SelectedRows[0].Index) ||
                        (style == ShiftStyle.BeforeSelected && i <= gridASS.SelectedRows[0].Index))
                    {

                        if (isSum)
                        {
                            lass.t_inicial.sumaTiempo(toShift);
                            lass.t_final.sumaTiempo(toShift);
                        }
                        else
                        {
                            lass.t_inicial.restaTiempo(toShift);
                            lass.t_final.restaTiempo(toShift);
                        }
                    }
                }
            }
            updateGridWithArrayList(al);
            updatePreviewAVS();
        }

        public void doConcat(int threshold, ArrayList estilos)
        {
            IsModified = true;

            UndoRedo.AddUndo(script, "Concatenar líneas de tiempos próximos"); //UndoStack.Push(PrepareForPush(al));

            ArrayList sort_al = new ArrayList();
            for (int i = 0; i < al.Count; i++)
            {
                sort_al.Add(new lineaASSidx(al[i].ToString(), i));
            }

            sort_al.Sort();


            lineaASSidx actual = null, siguiente = null;
            for (int i = 0; i < sort_al.Count - 1; i++)
            {
                actual = (lineaASSidx)sort_al[i];
                siguiente = (lineaASSidx)sort_al[i + 1];

                bool styleOK = false;

                foreach (estiloV4 a in estilos)
                    if (actual.estilo.Equals(a.Name)) styleOK = true;

                if (!styleOK) goto noMeMires;


                if (siguiente.t_inicial.getTiempo() > actual.t_final.getTiempo())
                {

                    double bleh = threshold; // increible lo tonta que es esta cosa a veces
                    double msToSeg = bleh / 1000;
                    double actThres = actual.t_final.getTiempo() + msToSeg;
                    double sig = siguiente.t_inicial.getTiempo();

                    if (actThres >= sig)
                    {
                        actual.t_final.setTiempo(siguiente.t_inicial.getTiempo());

                        al[actual.Index] = actual;

                        gridASS["tInicial", actual.Index].Value = actual.t_inicial.ToString();
                        gridASS["tFinal", actual.Index].Value = actual.t_final.ToString();
                    }
                }
            noMeMires:
                ;
            }
            updatePreviewAVS();
        }

        public void doOverlap(int threshold, ArrayList estilos)
        {
            UndoRedo.AddUndo(script, "Ajustar overlaps de tiempos próximos"); //UndoStack.Push(PrepareForPush(al));

            IsModified = true;
            ArrayList sort_al = new ArrayList();
            for (int i = 0; i < al.Count; i++)
            {
                sort_al.Add(new lineaASSidx(al[i].ToString(), i));
            }

            sort_al.Sort();


            lineaASSidx actual = null, siguiente = null;
            for (int i = 0; i < sort_al.Count - 1; i++)
            {
                actual = (lineaASSidx)sort_al[i];
                siguiente = (lineaASSidx)sort_al[i + 1];

                bool styleOK = false, styleOK2 = false; ;

                foreach (estiloV4 a in estilos)
                {
                    if (actual.estilo.Equals(a.Name)) styleOK = true;
                    if (siguiente.estilo.Equals(a.Name)) styleOK = true;
                }

                if (!styleOK) goto noMeGustaTuCara;

                double bleh = threshold;
                double ms = bleh / 1000;

                if (isInRange(actual.t_inicial.getTiempo(), siguiente.t_inicial.getTiempo(), ms))
                    actual.t_inicial.setTiempo(siguiente.t_inicial.getTiempo());

                if (isInRange(actual.t_final.getTiempo(), siguiente.t_final.getTiempo(), ms))
                    actual.t_final.setTiempo(siguiente.t_final.getTiempo());

                al[actual.Index] = actual;

                if (!styleOK2) goto noMeGustaTuCara;

                if (isInRange(actual.t_final.getTiempo(), siguiente.t_inicial.getTiempo(), ms))
                    siguiente.t_inicial.setTiempo(actual.t_final.getTiempo());

                al[siguiente.Index] = siguiente;

            noMeGustaTuCara:

                gridASS["tInicial", actual.Index].Value = actual.t_inicial.ToString();
                gridASS["tFinal", actual.Index].Value = actual.t_final.ToString();
            }
            updatePreviewAVS();
        }

        private bool isInRange(double original, double value, double range)
        {
            if (value - range <= original && original <= value + range) return true;
            return false;
        }

        public void doDetectCollision()
        {
            IsModified = true;
            ArrayList sort_al = new ArrayList();
            for (int i = 0; i < al.Count; i++)
            {
                sort_al.Add(new lineaASSidx(al[i].ToString(), i));
            }

            sort_al.Sort();

            for (int i = 0; i < sort_al.Count; i++)
            {
                bool col = false;

                lineaASSidx anterior = null;
                lineaASSidx actual = (lineaASSidx)sort_al[i];
                if (i > 0)
                {
                    anterior = (lineaASSidx)sort_al[i - 1];
                    if (anterior.t_final.getTiempo() > actual.t_inicial.getTiempo()) col = true;

                    if (col)
                    {
                        for (int x = 0; x < gridASS.Columns.Count; x++)
                        {
                            gridASS[x, actual.Index].Style.BackColor = Color.LightSteelBlue;
                            gridASS[x, anterior.Index].Style.BackColor = Color.LightSteelBlue;
                        }
                    }
                    else for (int x = 0; x < gridASS.Columns.Count; x++)
                            gridASS[x, actual.Index].Style.BackColor = RowBack;

                }

            }
        }

        public void doDetectSubtitleProblems()
        {
            int secChars = 17;
            try
            {
                secChars = int.Parse(getFromConfigFile("mainW_SecChars"));
            }
            catch { }

            int subChars = 76;
            try
            {
                subChars = int.Parse(getFromConfigFile("mainW_SubChars"));
            }
            catch { }

            int lineChars = 38;
            try
            {
                lineChars = int.Parse(getFromConfigFile("mainW_LineChars"));
            }
            catch { }

            for (int i = 0; i < al.Count; i++)
            {
                lineaASS lass = (lineaASS)al[i];

                bool hasWarnings = false;
                // Codigo para detectar errores

                if (!lass.IsComment())
                {
                    
                    string cleanText = lineaASS.cleanText(lass.texto);

                    // Comprobación de los caracteres por segundo
                    
                    double duration = lass.t_final.getTiempo() - lass.t_inicial.getTiempo();
                    double limitMaxPerSecond = duration * secChars;
                    if (cleanText.Length > limitMaxPerSecond)
                    {
                        hasWarnings = true;                       
                    }

                    // Comprobación de máximo de caracteres por subtítulo
                    
                    if (cleanText.Length > subChars)
                    {
                        hasWarnings = true;
                    }

                    // Comprobación de máximo de caracteres por línea
                   
                    string[] lines = lass.texto.Split(new string[] { "\\N" }, StringSplitOptions.RemoveEmptyEntries);

                    if (lines.Length > 1)
                    {
                        
                        foreach (string line in lines)
                        {
                            string cleanLine = lineaASS.cleanText(line);
                            
                            if (cleanLine.Length > lineChars)
                            {
                                hasWarnings = true;
                                break;
                            }
                        }
                    }

                    if (lines.Length > 2)
                    {
                        hasWarnings = true;
                    }

                }
                else
                {
                    hasWarnings = false;
                }

                for (int x = 0; x < gridASS.Columns.Count; x++)
                {
                    gridASS[x, i].Style.BackColor = (hasWarnings ? Color.LightPink : RowBack);
                }
            }
        }

        public void refreshGrid()
        {
            //al.TrimToSize(); // <- wtf ? puto codigo arcaico xDD
            
            //al = script.GetLines();
            gridASS.RowCount = al.Count;
            for (int i = 0; i < al.Count; i++)
            {
                lineaASS lass = (lineaASS)al[i];
                gridASS["Layer", i].Value = lass.colision.ToString();
                gridASS["tInicial", i].Value = lass.t_inicial.ToString();
                gridASS["tFinal", i].Value = lass.t_final.ToString();
                gridASS["Estilo", i].Value = lass.estilo;
                gridASS["Personaje", i].Value = lass.personaje;
                gridASS["Texto", i].Value = lass.texto;
                gridASS["Indice", i].Value = (i + 1) + script.GetLineArrayList().LineArrayIndex * LineArrayList.LineArrayListMax;

                gridASS["mLeft", i].Value = lineaASS.i2ns(lass.izquierda,4).ToString();
                gridASS["mRight", i].Value = lineaASS.i2ns(lass.derecha, 4).ToString();
                gridASS["mVert", i].Value = lineaASS.i2ns(lass.vertical, 4).ToString();

                Color f = RowFore;
                Color b = RowBack;

                if (lass.clase.Equals("Comment",StringComparison.InvariantCultureIgnoreCase))
                {
                    f = CommentFore;
                    b = CommentBack;
                }

                for (int x = 0; x < gridASS.ColumnCount; x++)
                {
                    gridASS[x, i].Style.ForeColor = f;
                    gridASS[x, i].Style.BackColor = b;
                }

                // casos especiales

                double ini = lass.t_inicial.getTiempo();
                double fin = lass.t_final.getTiempo();

                if (ini >= fin)
                {
                    gridASS["tInicial", i].Style.ForeColor = Color.OrangeRed;
                    gridASS["tFinal", i].Style.ForeColor = Color.OrangeRed;
                }

                if (ini == 0.0)
                    gridASS["tInicial", i].Style.ForeColor = Color.Gray;
                if (fin == 0.0)
                    gridASS["tFinal", i].Style.ForeColor = Color.Gray;


                if (lass.izquierda != 0)
                    gridASS["mLeft", i].Style.ForeColor = Color.Tomato;
                if (lass.derecha != 0)
                    gridASS["mRight", i].Style.ForeColor = Color.Tomato;
                if (lass.vertical != 0)
                    gridASS["mVert", i].Style.ForeColor = Color.Tomato;

                gridASS["Layer",i].Style.ForeColor = ((lass.colision % 2) == 0)? Color.Black : Color.Crimson;

            }
        }

        private lineaASS adjToKeyF(lineaASS lass, int initPre, int initPost, int finalPre, int finalPost, ArrayList estilos, out int anterior, out int posterior)
        {
            bool styleOK = false;

            UndoRedo.AddUndo(script, "Ajustar tiempos a KeyFrames"); //UndoStack.Push(PrepareForPush(al));

            foreach (estiloV4 a in estilos)
                if (lass.estilo.Equals(a.Name)) styleOK = true;

            anterior = 0; posterior = 0;

            if (!styleOK) return lass;

            int inFrame = Convert.ToInt32(Math.Round(lass.t_inicial.getTiempo() * videoInfo.FrameRate));
            int outFrame = Convert.ToInt32(Math.Round(lass.t_final.getTiempo() * videoInfo.FrameRate));

            int inRange = Math.Max(inFrame - initPre,0); // evitando negatiffos
            int inRangeEnd = inFrame + initPost;
            int outRange = outFrame - finalPre;
            int outRangeEnd = outFrame + finalPost;

            // lista de los keyframes que cumplen los requisitos
            ArrayList keysInRangeInit = new ArrayList();
            ArrayList keysInRangeFinal = new ArrayList();

            for (int i = 0; i < videoInfo.KeyFrames.Count; i++)
            {
                int k = (int)videoInfo.KeyFrames[i];
                if ((k >= inRange) && (k <= inRangeEnd)) keysInRangeInit.Add(k);
                if ((k <= outRangeEnd) && (k >= outRange)) keysInRangeFinal.Add(k);

                if (k <= inFrame)
                    anterior = k;

                if (k >= outFrame)
                    if (posterior == 0)
                        posterior = k;
            }

            if (keysInRangeInit.Count > 0)
            {

                double s = (Convert.ToDouble((int)keysInRangeInit[0]) / (double)videoInfo.FrameRate);
                lass.t_inicial.setTiempo(s - 0.01);
            }

            if (keysInRangeFinal.Count > 0)
            {
                double s = (Convert.ToDouble((int)keysInRangeFinal[keysInRangeFinal.Count - 1]) / (double)videoInfo.FrameRate);
                lass.t_final.setTiempo(s - 0.01);
            }

            return lass;
        }

        public void adjToKeyF(int initPre, int initPost, int finalPre, int finalPost, ArrayList estilos)
        {
            IsModified = true;

            ArrayList sort_al = new ArrayList();
            for (int i = 0; i < al.Count; i++)
            {
                sort_al.Add(new lineaASSidx(al[i].ToString(), i));
            }

            sort_al.Sort();

            int idx = 0;
            foreach (lineaASSidx lass in sort_al)
            {
                int a, p;
                lineaASS nass = adjToKeyF(lass, initPre, initPost, finalPre, finalPost, estilos, out a, out p);

                //gridASS["tInicial", lass.Index].Value = nass.t_inicial.ToString();
                //gridASS["tFinal", lass.Index].Value = nass.t_final.ToString();
                al[lass.Index] = nass;

                idx++;
            }
            updatePreviewAVS();
            updateGridWithArrayList(al);
        }

        // ya lo usara alguien xD

        private Tiempo GetFrameTime(int f)
        {
            if (frameTime == null) return null;
            if (frameTime.Count == 0) return null;

            if (!frameTime.ContainsKey(f))
                return null;
            else 
                return  (Tiempo)frameTime[f];
        }

        private Object _lock = new Object();

        private void CommentDialogue()
        {
            lock (_lock)
            {
                IsModified = true;
                if (gridASS.SelectedRows.Count == 0) return;
                lineaASS lass = null;
                bool isC = ComentarDescomentar.BackColor.Equals(CommentBack); //checkComment.Checked;

                for (int j = 0; j < gridASS.SelectedRows.Count; j++)
                {
                    int i = (gridASS.SelectedRows[j].Index);
                    //if (i >= gridASS.SelectedRows.Count) break;

                    try
                    {
                        lass = (lineaASS) script.GetLines()[i];                        
                        lass.clase = (isC) ? "Comment" : "Dialogue";

                        for (int x = 0; x < gridASS.Columns.Count; x++)
                        {
                            gridASS[x, i].Style.ForeColor = (isC) ? CommentFore : RowFore;
                            selectedLine.BackColor = gridASS[x, i].Style.BackColor = (isC) ? CommentBack : RowBack;
                        }
                    }
                    catch
                    {
                    }
                }

                updateGridWithArrayList(script.GetLines());
                //updatePreviewAVS();

                if (isAudioLoaded)
                    updateAudioGrid();
            }
        }

    }
}
