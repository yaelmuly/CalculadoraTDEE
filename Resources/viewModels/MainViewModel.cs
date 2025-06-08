using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CalculadoraTDEE.Resources.models;
using CalculadoraTDEE.Resources.utils;

namespace CalculadoraTDEE.viewModels
{
    public class PacienteViewModel : INotifyPropertyChanged
    {
        private string _nombre;
        private string _apellido;
        private string _edad;
        private string _peso;
        private string _estatura;
        private int _sexoIndex;
        private int _actividadIndex;

        public ObservableCollection<Paciente> Pacientes { get; } = new ObservableCollection<Paciente>();

        public string Nombre
        {
            get => _nombre;
            set => SetProperty(ref _nombre, value);
        }

        public string Apellido
        {
            get => _apellido;
            set => SetProperty(ref _apellido, value);
        }

        public string Edad
        {
            get => _edad;
            set => SetProperty(ref _edad, value);
        }

        public string Peso
        {
            get => _peso;
            set => SetProperty(ref _peso, value);
        }

        public string Estatura
        {
            get => _estatura;
            set => SetProperty(ref _estatura, value);
        }

        public int SexoIndex
        {
            get => _sexoIndex;
            set => SetProperty(ref _sexoIndex, value);
        }

        public int ActividadIndex
        {
            get => _actividadIndex;
            set => SetProperty(ref _actividadIndex, value);
        }

        public ICommand RegistrarCommand { get; }

        public PacienteViewModel()
        {
            RegistrarCommand = new ComandoBasico(RegistrarPaciente);
        }

        private void RegistrarPaciente()
        {
            var validacion = Paciente.ValidarDatos(Nombre, Apellido, Edad, Peso, Estatura, SexoIndex, ActividadIndex);
            if (!validacion.EsValido)
            {
                // Aquí podrías mostrar un mensaje de error en pantalla
                return;
            }

            var paciente = new Paciente
            {
                Nombre = Nombre,
                Apellido = Apellido,
                Edad = int.Parse(Edad),
                Peso = double.Parse(Peso),
                Estatura = double.Parse(Estatura),
                SexoIndex = SexoIndex,
                ActividadIndex = ActividadIndex
            };


            Pacientes.Add(paciente);
            LimpiarFormulario();
        }

        private void LimpiarFormulario()
        {
            Nombre = "";
            Apellido = "";
            Edad = "";
            Peso = "";
            Estatura = "";
            SexoIndex = -1;
            ActividadIndex = -1;
        }

        private double ObtenerNivelActividad(int index)
        {
            double[] niveles = { 1.2, 1.375, 1.55, 1.725, 1.9 };
            return index >= 0 && index < niveles.Length ? niveles[index] : 1.0;
        }

        private string ObtenerDescripcionActividad(int index)
        {
            string[] descripciones = {
                "Sedentario (poco o ningún ejercicio)",
                "Ligera (ejercicio ligero 1–3 días/semana)",
                "Moderada (ejercicio moderado 3–5 días/semana)",
                "Intensa (ejercicio fuerte 6–7 días/semana)",
                "Muy intensa (entrenamiento físico diario o dos veces al día)"
            };
            return index >= 0 && index < descripciones.Length ? descripciones[index] : "Desconocido";
        }

        // Implementación de INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propiedad = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propiedad));
        }

        protected bool SetProperty<T>(ref T campo, T valor, [CallerMemberName] string propiedad = null)
        {
            if (EqualityComparer<T>.Default.Equals(campo, valor)) return false;
            campo = valor;
            OnPropertyChanged(propiedad);
            return true;
        }
    }


}
