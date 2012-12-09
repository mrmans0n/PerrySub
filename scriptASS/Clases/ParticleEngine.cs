using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace scriptASS.Clases
{
    class ParticleEngine
    {
        private ArrayList world;
        private Stack pendientes;
        private Stack animaciones;
        private double time;
        private double trate;
        private double brate;
        private string[] formas= new string[10]; 
        public Queue lineasASS = new Queue();

        public ParticleEngine()
        {
            world = new ArrayList();
            pendientes = new Stack();
            animaciones = new Stack();
            time = 0.0;
            trate = 1.0;
            brate = 2.0;
        }

        public ParticleEngine(double t, double r, double b)
        {
            world = new ArrayList();
            pendientes = new Stack();
            animaciones = new Stack();
            time = t;
            trate = r;
            brate = b;
        }

        public ParticleEngine(double t, double r, double b, string[] particula)
        {
            world = new ArrayList();
            pendientes = new Stack();
            animaciones = new Stack();
            formas = particula;
            time = t;
            trate = r;
            brate = b;
        }

        //Empezamos con una version sencilla el main llevará el control de las iteraciones del motor
        //El motor solo ha de preocuparse por crear las particulas
        public void Itera()
        {
            int particulas = world.Count;

            if (time * brate <= particulas)
            {
                for (int i = 0; i < brate; i++)
                {
                    //Ejemplo más adelante se podrá modificar pudiendo cambiar la particula
                    SSAParticle particula = new SSAParticle(time);
                    if (formas.Length>0 && formas[0]!=null)
                    {
                        particula.SetForma(formas); 
                    }
                    
                    pendientes.Push(particula);
                }
            }
            Anima();
            Procesa();
            time += trate;

        }

        private void Anima()
        {
            for (int i = 0; i < pendientes.Count; i++)
            {
                SSAParticle particula = (SSAParticle)pendientes.Pop();
                Stack aniBkp = (Stack)animaciones.Clone();
                Modifica(ref particula);
                animaciones = (Stack)aniBkp.Clone();
                aniBkp.Clear();
                int partX=particula.GetPos().X;
                int partY=particula.GetPos().Y;
                bool inner=partX<1300&&partX>-10&&partY>-10&&partY<1100;
                if (particula.GetLife()>0 && inner)
                    world.Add(particula);
            }
        }

        private lineaASS ParticulaToLinea(SSAParticle particula,int i)
        {
            lineaASS l = new lineaASS();
            Tiempo ti = new Tiempo();
            Tiempo tf = new Tiempo();
            ti.setTiempo(time);
            tf.setTiempo(time + trate);
            l.t_inicial = ti;
            l.t_final = tf;
            l.texto = particula.Show(time)[i];
            return l;
        }

        private void Procesa()
        {
            IEnumerator particulas = world.GetEnumerator();
            SSAParticle particula;
            while (particulas.MoveNext())
            {
                particula = (SSAParticle)particulas.Current;
                for (int i = 0; i < particula.NumLineas(); i++)
                {
                    lineaASS l = ParticulaToLinea(particula, i);
                    if (l.texto!=null)
                        lineasASS.Enqueue(l);
                }
                if (particula.GetBirth() + particula.GetLife() > time)
                    pendientes.Push(particula);
            }

            
        }

        private void Modifica(ref SSAParticle p)
        {
            while(animaciones.Count>0)
            {
                ((ParticleAnimacion)animaciones.Pop()).ApplyAnimacion(ref p);
            }
        }

        public void AddAnimacion(ParticleAnimacion pa)
        {
            animaciones.Push(pa);
        }

    }
}
