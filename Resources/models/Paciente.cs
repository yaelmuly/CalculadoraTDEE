namespace CalculadoraTDEE.Resources.models
{
    public class ValidacionResultado
    {
        public bool EsValido { get; set; }
        public string Mensaje { get; set; }
    }
    public class Paciente
    {
        public string Nombre { get; set; } = "";
        public string Apellido { get; set; } = "";
        public int Edad { get; set; }
        public double Peso { get; set; }
        public double Estatura { get; set; } // En cm
        public int SexoIndex { get; set; } // 0: Masculino, 1: Femenino
        public int ActividadIndex { get; set; }

        public string Sexo => SexoIndex == 0 ? "Masculino" : "Femenino";

        public double IMC => Math.Round(Peso / Math.Pow(Estatura / 100, 2), 2);

        public string ClasificacionIMC
        {
            get
            {
                if (IMC < 18.5) return "Bajo peso";
                else if (IMC < 25.0) return "Peso normal";
                else if (IMC < 30.0) return "Sobrepeso";
                else if (IMC < 35.0) return "Obesidad clase I";
                else if (IMC < 40.0) return "Obesidad clase II";
                else return "Obesidad clase III";
            }
        }

        public double PorcentajeGrasaCorporal =>
            Math.Round(1.2 * IMC + 0.23 * Edad - 10.8 * (SexoIndex == 0 ? 1 : 0) - 5.4, 2);

        public string ClasificacionGrasa
        {
            get
            {
                double gc = PorcentajeGrasaCorporal;
                if (SexoIndex == 0) // Hombre
                {
                    if (gc <= 5) return "Grasa esencial";
                    if (gc <= 13) return "Atleta";
                    if (gc <= 17) return "Fitness";
                    if (gc <= 24) return "Aceptable";
                    return "Obesidad";
                }
                else // Mujer
                {
                    if (gc <= 13) return "Grasa esencial";
                    if (gc <= 20) return "Atleta";
                    if (gc <= 24) return "Fitness";
                    if (gc <= 31) return "Aceptable";
                    return "Obesidad";
                }
            }
        }

        public double PesoIdeal
        {
            get
            {
                if (SexoIndex == 0) // Masculino
                    return Math.Round(Estatura - 100 - ((Estatura - 150) / 4), 2);
                else // Femenino
                    return Math.Round(Estatura - 100 - ((Estatura - 150) / 2.5), 2);
            }
        }

        public double BMR
        {
            get
            {
                double baseValue = (Estatura * 6.25) + (Peso * 9.99) - (Edad * 4.92);
                return SexoIndex == 0
                    ? Math.Round(baseValue + 5, 2)
                    : Math.Round(baseValue - 161, 2);
            }
        }

        public double NivelActividad
        {
            get
            {
                double[] factores = { 1.2, 1.375, 1.55, 1.725, 1.9 };
                return ActividadIndex >= 0 && ActividadIndex < factores.Length ? factores[ActividadIndex] : 1.2;
            }
        }

        public double TDEE => Math.Round(BMR * NivelActividad, 2);
    

    public static ValidacionResultado ValidarDatos(string nombre, string apellido, string edad, string peso, string estatura, int sexoIndex, int actividadIndex)
        {
            if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(apellido))
                return new ValidacionResultado { EsValido = false, Mensaje = "Nombre y apellido son obligatorios." };

            if (!int.TryParse(edad, out int e) || e <= 0)
                return new ValidacionResultado { EsValido = false, Mensaje = "Edad inválida." };

            if (!double.TryParse(peso, out double p) || p <= 0)
                return new ValidacionResultado { EsValido = false, Mensaje = "Peso inválido." };

            if (!double.TryParse(estatura, out double est) || est <= 0)
                return new ValidacionResultado { EsValido = false, Mensaje = "Estatura inválida." };

            if (sexoIndex < 0 || sexoIndex > 1)
                return new ValidacionResultado { EsValido = false, Mensaje = "Selecciona un sexo." };

            if (actividadIndex < 0 || actividadIndex > 4)
                return new ValidacionResultado { EsValido = false, Mensaje = "Selecciona un nivel de actividad." };

            return new ValidacionResultado { EsValido = true };
        }
    }

}
