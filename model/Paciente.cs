using System.ComponentModel;
using System.Linq; // ← AGREGADO: Necesario para usar .Any()

namespace CalculadoraTDEE.model
{
    public class Paciente : INotifyPropertyChanged
    {
        private string _nombre;
        private string _apellido;
        private int _edad;
        private double _peso;
        private double _estatura;
        private string _sexo;
        private double _nivelActividad;
        private string _descripcionActividad;

        // Propiedades básicas
        public string Nombre
        {
            get => _nombre;
            set
            {
                _nombre = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(NombreCompleto));
                OnPropertyChanged(nameof(InfoBasica));
            }
        }

        public string Apellido
        {
            get => _apellido;
            set
            {
                _apellido = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(NombreCompleto));
                OnPropertyChanged(nameof(InfoBasica));
            }
        }

        public int Edad
        {
            get => _edad;
            set
            {
                _edad = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(InfoBasica));
                RecalcularValores();
            }
        }

        public double Peso
        {
            get => _peso;
            set
            {
                _peso = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(InfoBasica));
                RecalcularValores();
            }
        }

        public double Estatura
        {
            get => _estatura;
            set
            {
                _estatura = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(InfoBasica));
                RecalcularValores();
            }
        }

        public string Sexo
        {
            get => _sexo;
            set
            {
                _sexo = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(InfoBasica));
                RecalcularValores();
            }
        }

        public double NivelActividad
        {
            get => _nivelActividad;
            set
            {
                _nivelActividad = value;
                OnPropertyChanged();
                RecalcularValores();
            }
        }

        public string DescripcionActividad
        {
            get => _descripcionActividad;
            set
            {
                _descripcionActividad = value;
                OnPropertyChanged();
            }
        }

        // Propiedades calculadas
        public double IMC => Peso / Math.Pow(Estatura / 100, 2);

        public string ClasificacionIMC
        {
            get
            {
                var imc = IMC;
                if (imc < 18.5) return "Bajo peso";
                if (imc <= 24.9) return "Peso normal";
                if (imc <= 29.9) return "Pre-obesidad o Sobrepeso";
                if (imc <= 34.9) return "Obesidad clase I";
                if (imc <= 39.9) return "Obesidad clase II";
                return "Obesidad clase III";
            }
        }

        public double PorcentajeGrasaCorporal
        {
            get
            {
                var sexoMultiplicador = Sexo == "Masculino" ? 1 : 0;
                return 1.2 * IMC + 0.23 * Edad - 10.8 * sexoMultiplicador - 5.4;
            }
        }

        public string ClasificacionGrasaCorporal
        {
            get
            {
                var porcentaje = PorcentajeGrasaCorporal;
                var esMasculino = Sexo == "Masculino";

                if (esMasculino)
                {
                    if (porcentaje <= 5) return "Grasa esencial";
                    if (porcentaje <= 13) return "Atletas";
                    if (porcentaje <= 17) return "Fitness";
                    if (porcentaje <= 24) return "Aceptable";
                    return "Obesidad";
                }
                else
                {
                    if (porcentaje <= 13) return "Grasa esencial";
                    if (porcentaje <= 20) return "Atletas";
                    if (porcentaje <= 24) return "Fitness";
                    if (porcentaje <= 31) return "Aceptable";
                    return "Obesidad";
                }
            }
        }

        public double PesoIdeal
        {
            get
            {
                if (Sexo == "Masculino")
                {
                    return Estatura - 100 - ((Estatura - 150) / 4);
                }
                else
                {
                    return Estatura - 100 - ((Estatura - 150) / 2.5);
                }
            }
        }

        public double BMR
        {
            get
            {
                var baseValue = (Estatura * 6.25) + (Peso * 9.99) - (Edad * 4.92);
                return Sexo == "Masculino" ? baseValue + 5 : baseValue - 161;
            }
        }

        public double TDEE => BMR * NivelActividad;

        // Propiedades para la interfaz
        public string NombreCompleto => $"{Nombre} {Apellido}";

        public string InfoBasica => $"{Edad} años, {Peso}kg, {Estatura}cm";

        public string ResumenCalculos =>
            $"IMC: {IMC:F1} ({ClasificacionIMC})\n" +
            $"% Grasa: {PorcentajeGrasaCorporal:F1}% ({ClasificacionGrasaCorporal})\n" +
            $"Peso Ideal: {PesoIdeal:F1} kg\n" +
            $"BMR: {BMR:F0} cal/día\n" +
            $"TDEE: {TDEE:F0} cal/día";

        // Validaciones
        public static ValidationResult ValidarDatos(string nombre, string apellido, string edad,
            string peso, string estatura, int sexoIndex, int actividadIndex)
        {
            var result = new ValidationResult();

            // Validar nombre
            if (string.IsNullOrWhiteSpace(nombre))
            {
                result.Errores.Add("Nombre", "El nombre es requerido");
            }
            else if (nombre.Length < 2)
            {
                result.Errores.Add("Nombre", "El nombre debe tener al menos 2 caracteres");
            }

            // Validar apellido
            if (string.IsNullOrWhiteSpace(apellido))
            {
                result.Errores.Add("Apellido", "El apellido es requerido");
            }
            else if (apellido.Length < 2)
            {
                result.Errores.Add("Apellido", "El apellido debe tener al menos 2 caracteres");
            }

            // Validar edad
            if (!int.TryParse(edad, out int edadInt))
            {
                result.Errores.Add("Edad", "La edad debe ser un número válido");
            }
            else if (edadInt < 1 || edadInt > 120)
            {
                result.Errores.Add("Edad", "La edad debe estar entre 1 y 120 años");
            }

            // Validar peso
            if (!double.TryParse(peso, out double pesoDouble))
            {
                result.Errores.Add("Peso", "El peso debe ser un número válido");
            }
            else if (pesoDouble < 20 || pesoDouble > 300)
            {
                result.Errores.Add("Peso", "El peso debe estar entre 20 y 300 kg");
            }

            // Validar estatura
            if (!double.TryParse(estatura, out double estaturaDouble))
            {
                result.Errores.Add("Estatura", "La estatura debe ser un número válido");
            }
            else if (estaturaDouble < 100 || estaturaDouble > 250)
            {
                result.Errores.Add("Estatura", "La estatura debe estar entre 100 y 250 cm");
            }

            // Validar sexo
            if (sexoIndex < 0)
            {
                result.Errores.Add("Sexo", "Debe seleccionar un sexo");
            }

            // Validar actividad
            if (actividadIndex < 0)
            {
                result.Errores.Add("Actividad", "Debe seleccionar un nivel de actividad");
            }

            result.EsValido = !result.Errores.Any(); // ← CORREGIDO: Ahora funciona con using System.Linq;
            return result;
        }

        private void RecalcularValores()
        {
            OnPropertyChanged(nameof(IMC));
            OnPropertyChanged(nameof(ClasificacionIMC));
            OnPropertyChanged(nameof(PorcentajeGrasaCorporal));
            OnPropertyChanged(nameof(ClasificacionGrasaCorporal));
            OnPropertyChanged(nameof(PesoIdeal));
            OnPropertyChanged(nameof(BMR));
            OnPropertyChanged(nameof(TDEE));
            OnPropertyChanged(nameof(ResumenCalculos));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ValidationResult
    {
        public bool EsValido { get; set; }
        public Dictionary<string, string> Errores { get; set; } = new Dictionary<string, string>();
    }
}