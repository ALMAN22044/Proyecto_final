using Api.Models;
using System;
using System.Runtime.Intrinsics.X86;

namespace Api.Date
{
    public class XDificultad
    {
        // Método principal que calcula la dificultad de las asignaturas en función del trimestre y el pensum
        public List<MPresu> Dificultad(int NTrimestre, List<MPresu> Pensum)
        {
            var list = new List<MPresu>(); // Lista final de asignaturas procesadas
            var aux = new List<MPresu>(); // Lista auxiliar para almacenamiento temporal
            var faltantes = new List<MPresu>(); // Lista de asignaturas que no cumplen los requisitos en el momento

            // Array para manejar información sobre correquisitos
            object[,] siguiente = new object[1, 3] {
                { false, "", false }
            };

            int suma = 0; // Suma de créditos en el trimestre actual
            int tri = 0; // Trimestre actual

            // Calcula el máximo de créditos permitidos por trimestre en función del número de trimestres
            int maximo_de_creditos = (int)Math.Round((double)Pensum.Sum(x => x.Creditos_Asignatura) / (NTrimestre == 14 ? 14 : NTrimestre + (NTrimestre > 19 ? NTrimestre - 5 : NTrimestre > 16 ? NTrimestre - 8 : NTrimestre - 10)));
            int creditoaux = 0;
            foreach (var item in Pensum)
            {
                // Reinicia la suma de créditos si cambia de trimestre
                if (tri != item.Trimestre)
                {
                    suma = 0;
                    tri = item.Trimestre;
                }

                // Añadir directamente las asignaturas del primer trimestre
                if (item.Trimestre == 1)
                {
                    aux.Add(item);
                    continue;
                }

                // Ajusta los créditos de asignaturas con código específico si no tienen créditos definidos
                if (item.Creditos_Asignatura == 0 && item.Codigo_Asignatura.Length < 7)
                {
                    if (item.Codigo_Asignatura.Substring(3, 1) == "3")
                        creditoaux = 4;
                    else if (item.Codigo_Asignatura.Substring(3, 1) == "2" || item.Codigo_Asignatura.Substring(3, 1) == "1")
                        creditoaux = 2;
                }

                // Verifica si la asignatura tiene correquisitos
                if ((bool)siguiente[0, 0] == true && item.Codigo_Asignatura.Contains((string)siguiente[0, 1]))
                {
                    if ((bool)siguiente[0, 2])
                    {
                        item.Total_Creditos = creditoaux + suma;
                        suma = item.Total_Creditos;
                        aux.Add(item);
                    }
                    else
                        faltantes.Add(item);

                    siguiente[0, 0] = false;
                    siguiente[0, 1] = "";
                }
                else
                {
                    // Verifica si la asignatura cumple los requisitos para ser cursada
                    if (requerimientos(item, Pensum, item.Trimestre, suma, maximo_de_creditos))
                    {
                        item.Total_Creditos = creditoaux + suma;
                        suma = item.Total_Creditos;
                        aux.Add(item);
                        siguiente[0, 2] = true;
                    }
                    else
                    {
                        // Si hay espacio en el trimestre, se agrega; si no, va a faltantes
                        if (suma + item.Creditos_Asignatura <= maximo_de_creditos)
                        {
                            item.Total_Creditos = creditoaux + suma;
                            suma = item.Total_Creditos;
                            aux.Add(item);
                            siguiente[0, 2] = true;
                        }
                        else
                        {
                            faltantes.Add(item);
                            siguiente[0, 2] = false;
                        }
                    }

                    // Verifica si la asignatura tiene correquisitos
                    if (correquisito(item, Pensum, item.Trimestre))
                    {
                        siguiente[0, 0] = true;
                        siguiente[0, 1] = item.Codigo_Asignatura;
                    }
                }

                // Intenta reasignar asignaturas en la lista de faltantes si es posible
                if (faltantes.Count > 0)
                {
                    List<MPresu> aEliminar = new List<MPresu>();

                    foreach (var dr in faltantes)
                    {
                        bool Forzado = true;
                        foreach (var j in faltantes)
                        {
                            if (dr.Correquisitos == j.Codigo_Asignatura)
                            {
                                dr.Trimestre = item.Trimestre;
                                Forzado = false;
                                break;
                            }
                        }
                        if (requerimientos(dr, Pensum, item.Trimestre, suma, maximo_de_creditos) && Forzado == true)
                        {
                            dr.Trimestre = item.Trimestre;
                            dr.Total_Creditos = creditoaux + suma;
                            suma = dr.Total_Creditos;
                            aux.Add(dr);
                            aEliminar.Add(dr);
                            break;
                        }
                    }
                    foreach (var i in aEliminar)
                        faltantes.Remove(i);
                }
            }

            // Asigna las asignaturas faltantes a los trimestres restantes
            int n = 14;
            tri = 14;

            if (faltantes.Count > 0)
            {
                Console.WriteLine(maximo_de_creditos);
                List<MPresu> aEliminar = new List<MPresu>();
                foreach (var dr in faltantes)
                {
                    bool Forzado = false;
                    dr.Trimestre = n;
                    if (tri != dr.Trimestre)
                    {
                        suma = 0;
                        tri = n;
                    }
                    if (correquisito(dr, faltantes, tri))
                        Forzado = true;

                    dr.Total_Creditos = creditoaux + suma;
                    suma = dr.Total_Creditos;
                    aux.Add(dr);
                    aEliminar.Add(dr);

                    if (suma >= maximo_de_creditos && Forzado == false && (NTrimestre != 20 || n != 20))
                        n++;

                }
                foreach (var i in aEliminar)
                    faltantes.Remove(i);

            }

            // Copia las asignaturas procesadas a la lista final
            list.AddRange(aux);

            return list.OrderBy(x => x.Trimestre).ThenBy(x => x.Codigo_Asignatura).ToList();
        }

        // Verifica si la asignatura cumple con los requisitos previos
        private bool requerimientos(MPresu item, List<MPresu> Pensum, int trimestreactual, int suma, int maximo)
        {
            bool resultado = false;
            bool forzado = false;
            foreach (var dr in Pensum)
            {
                if (dr.Trimestre > trimestreactual && dr.Prerequisitos != null && dr.Prerequisitos.Contains(item.Codigo_Asignatura))
                {
                    resultado = dr.Trimestre - trimestreactual <= 1;
                    forzado = resultado;
                    break;
                }
            }
            if (!forzado)
                resultado = (item.Dificultad == 1 || item.Dificultad == 2) && suma + item.Creditos_Asignatura <= maximo;
            return resultado;
        }

        private bool requerimientosP(MPresu item, List<MPresu> Pensum, int trimestreactual, decimal suma, decimal maximo)
        {
            bool resultado = false;
            bool forzado = false;
            foreach (var dr in Pensum)
            {
                if (dr.Trimestre > trimestreactual && dr.Prerequisitos != null && dr.Prerequisitos.Contains(item.Codigo_Asignatura) )
                {
                    resultado = dr.Trimestre - trimestreactual <= 1  ;
                    forzado = resultado;
                    break;
                }
            }
            if (!forzado)
                resultado = suma + item.Costo_Asignatura <= maximo;
            return resultado;
        }
        private bool requerimientosT(MPresu item, List<MPresu> Pensum, int trimestreactual, decimal suma, decimal maximo)
        {
            bool resultado = false;
            foreach (var dr in Pensum)
            {
                if (dr.Trimestre > trimestreactual && dr.Prerequisitos != null && dr.Prerequisitos.Contains(item.Codigo_Asignatura))
                {
                    resultado = dr.Trimestre - trimestreactual <= 1;
                    break;
                }
            }
            return resultado;
        }

        // Verifica si la asignatura tiene correquisitos en el trimestre actual
        private bool correquisito(MPresu item, List<MPresu> Pensum, int trimestreactual)
        {
            return Pensum.Any(dr => dr.Trimestre == trimestreactual && dr.Correquisitos == item.Codigo_Asignatura);
        }


        public List<MPresu> PresupuestoCliente(decimal presupuesto, List<MPresu> Pensum)
        {
            var list = new List<MPresu>(); // Lista final de asignaturas procesadas
            var aux = new List<MPresu>(); // Lista auxiliar para almacenamiento temporal
            var faltantes = new List<MPresu>(); // Lista de asignaturas que no cumplen los requisitos en el momento

            //// Array para manejar información sobre correquisitos
            object[,] siguiente = new object[1, 3] {
                { false, "", false }
            };

            decimal suma = 0; // Suma de créditos en el trimestre actual
            int tri = 1; // Trimestre actual

            // Calcula el máximo de créditos permitidos por trimestre en función del número de trimestres
            foreach (var item in Pensum.OrderBy(x => x.Trimestre).ThenByDescending(x => x.Costo_Asignatura))
            {

                if (suma + item.Costo_Asignatura < presupuesto && tri != item.Trimestre && presupuesto < 115000)
                {
                    foreach (var i in Pensum.Where(x => x.Trimestre > item.Trimestre && item.Trimestre > 1).OrderBy(x => x.Trimestre).ThenByDescending(x => x.Costo_Asignatura))
                    {
                        i.Trimestre -= 1;
                    }
                }
                // Reinicia la suma de créditos si cambia de trimestre
                if (tri != item.Trimestre)
                {
                    suma = 0;
                    tri = item.Trimestre;
                }

                // Añadir directamente las asignaturas del primer trimestre
                if (item.Trimestre == 1)
                {
                    aux.Add(item);
                    suma = item.Costo_Asignatura + suma;
                    Pensum.Remove(item);
                   
                    continue;
                }
                if (faltantes.Count > 0)
                {
                    List<MPresu> aEliminar = new List<MPresu>();

                    foreach (var dr in faltantes)
                    {
                        bool Forzado = true;
                        foreach (var j in faltantes)
                        {
                            if (dr.Correquisitos == j.Codigo_Asignatura)
                            {
                                dr.Trimestre = item.Trimestre;
                                Forzado = false;
                                break;
                            }
                        }
                        if (requerimientosP(dr, Pensum, item.Trimestre, suma, presupuesto) && Forzado == true)
                        {
                            dr.Trimestre = item.Trimestre;
                            suma = suma + dr.Costo_Asignatura;
                            aux.Add(dr);
                            Pensum.Remove(dr);
                            aEliminar.Add(dr);
                            break;
                        }
                    }
                    foreach (var i in aEliminar)
                        faltantes.Remove(i);
                }

                if (suma + item.Costo_Asignatura >= presupuesto && (bool)siguiente[0, 0] == false )
                {
                   
                    foreach (var i in Pensum.Where(x=> x.Trimestre == item.Trimestre).OrderBy(x => x.Trimestre).ThenByDescending(x => x.Costo_Asignatura))
                    {
                        i.Trimestre +=1; 
                    }
                    faltantes.Add(item);
                    continue;
                }

                // Verifica si la asignatura tiene correquisitos
                if ((bool)siguiente[0, 0] == true && item.Codigo_Asignatura.Contains((string)siguiente[0, 1]))
                {
                    if ((bool)siguiente[0, 2])
                    {
                        suma = item.Costo_Asignatura + suma;
                        aux.Add(item);
                        Pensum.Remove(item);
                    }
                    else
                        faltantes.Add(item);

                    siguiente[0, 0] = false;
                    siguiente[0, 1] = "";
                    continue;
                }

                else
                {
                    // Verifica si la asignatura cumple los requisitos para ser cursada
                    if (requerimientosP(item, Pensum, item.Trimestre, suma, presupuesto))
                    {
                        suma = suma + item.Costo_Asignatura;
                        Pensum.Remove(item);
                        aux.Add(item);
                        siguiente[0, 2] = true;
                    }
                    else
                    {


                        faltantes.Add(item);
                        siguiente[0, 2] = false;

                    }

                    // Verifica si la asignatura tiene correquisitos
                    if (correquisito(item, Pensum, item.Trimestre))
                    {
                        siguiente[0, 0] = true;
                        siguiente[0, 1] = item.Codigo_Asignatura;
                    }
                }

                // Intenta reasignar asignaturas en la lista de faltantes si es posible

            }

            // Asigna las asignaturas faltantes a los trimestres restantes
            int n = 14;
            tri = 14;

            if (faltantes.Count > 0)
            {
                Console.WriteLine(presupuesto);
                List<MPresu> aEliminar = new List<MPresu>();
                foreach (var dr in faltantes)
                {
                    bool Forzado = false;
                    dr.Trimestre = n;
                    if (tri != dr.Trimestre)
                    {
                        suma = 0;
                        tri = n;
                    }
                    if (correquisito(dr, faltantes, tri))
                        Forzado = true;


                    if (suma + dr.Costo_Asignatura >= presupuesto && Forzado == false)
                        n++;
                    else
                    {
                        suma = dr.Costo_Asignatura + suma;
                        Pensum.Remove(dr);
                        aux.Add(dr);
                        aEliminar.Add(dr);
                    }




                }
                foreach (var i in aEliminar)
                    faltantes.Remove(i);

            }

            //Copia las asignaturas procesadas a la lista final


            list.AddRange(aux);

            return list.OrderBy(x => x.Trimestre).ThenBy(x => x.Codigo_Asignatura).ToList();
        }
    }
}
