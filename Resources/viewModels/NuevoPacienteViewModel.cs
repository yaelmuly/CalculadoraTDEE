using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CalculadoraTDEE.Resources.models;
using CalculadoraTDEE.Resources.utils;

namespace CalculadoraTDEE.ViewModels
{
    public class NuevoPacienteViewModel : INotifyPropertyChanged
    {
        // Campos privados
        private string _nombre;
        private string _apellido;
        private string _edad;
        private string _peso;
        private string _estatura;
        private string _sexo;
        private string _nivelActividad;
        private string _mensaje;
        private bool _tieneErrores;

        // Propiedades públicas
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

        public string Sexo
        {
            get => _sexo;
            set => SetProperty(ref _sexo, value);
        }

        public string NivelActividad
        {
            get => _nivelActividad;
            set => SetProperty(ref _nivelActividad, value);
        }

        public string Mensaje
        {
            get => _mensaje;
            set => SetProperty(ref _mensaje, value);
        }

        public bool TieneErrores
        {
            get => _tieneErrores;
            set => SetProperty(ref _tieneErrores, value);
        }

        public Action<Paciente> PacienteCreadoCallback { get; set; }
        public Action CancelarCallback { get; set; }

        // Comandos
        public ICommand CalcularCommand { get; }
        public ICommand CancelarCommand { get; }

        public NuevoPacienteViewModel()
        {
            CalcularCommand = new ComandoBasico(Calcular);
            CancelarCommand = new ComandoBasico(Cancelar);
        }

        private void Calcular()
        {
            int sexoIndex = Sexo == "Masculino" ? 0 : Sexo == "Femenino" ? 1 : -1;
            int actividadIndex = ObtenerIndiceActividad(NivelActividad);

            var validacion = Paciente.ValidarDatos(Nombre, Apellido, Edad, Peso, Estatura, sexoIndex, actividadIndex);
            if (!validacion.EsValido)
            {
                TieneErrores = true;
                Mensaje = string.Join("\n", validacion.Errores.Values);
                return;
            }

            var paciente = new Paciente
            {
                Nombre = Nombre,
                Apellido = Apellido,
                Edad = int.Parse(Edad),
                Peso = double.Parse(Peso),
                Estatura = double.Parse(Estatura),
                Sexo = Sexo,
                NivelActividad = ObtenerMultiplicadorActividad(NivelActividad),
                DescripcionActividad = NivelActividad
            };

            PacienteCreadoCallback?.Invoke(paciente);

            Mensaje = $"Paciente {paciente.NombreCompleto} registrado correctamente.";
            TieneErrores = false;
        }

        private void Cancelar()
        {
            CancelarCallback?.Invoke();
        }

        private int ObtenerIndiceActividad(string nivel)
        {
            if (nivel.Contains("1.2")) return 0;
            if (nivel.Contains("1.375")) return 1;
            if (nivel.Contains("1.55")) return 2;
            if (nivel.Contains("1.725")) return 3;
            if (nivel.Contains("1.9")) return 4;
            return -1;
        }

        private double ObtenerMultiplicadorActividad(string nivel)
        {
            if (nivel.Contains("1.2")) return 1.2;
            if (nivel.Contains("1.375")) return 1.375;
            if (nivel.Contains("1.55")) return 1.55;
            if (nivel.Contains("1.725")) return 1.725;
            if (nivel.Contains("1.9")) return 1.9;
            return 1.2;
        }

        // Implementación INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propiedad = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propiedad));
        }

        protected bool SetProperty<T>(ref T campo, T valor, [CallerMemberName] string propiedad = null)
        {
            if (Equals(campo, valor)) return false;
            campo = valor;
            OnPropertyChanged(propiedad);
            return true;
        }
    }

}
