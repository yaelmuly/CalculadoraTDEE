using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CalculadoraTDEE.Resources.models;

public class PacienteViewModel : INotifyPropertyChanged
{
    public ObservableCollection<Paciente> Pacientes { get; set; } = new();

    public string Nombre { get; set; } = "";
    public string Apellido { get; set; } = "";
    public string Edad { get; set; } = "";
    public string Peso { get; set; } = "";
    public string Estatura { get; set; } = "";
    public int SexoIndex { get; set; }
    public int ActividadIndex { get; set; }

    public Paciente? PacienteSeleccionado { get; set; }

    public ICommand RegistrarCommand => new Command(Registrar);
    public ICommand EliminarCommand => new Command<Paciente>(Eliminar);
    public ICommand EditarCommand => new Command<Paciente>(Editar);

    void Registrar()
    {
        if (double.TryParse(Peso, out double peso) && double.TryParse(Estatura, out double estatura) && int.TryParse(Edad, out int edad))
        {
            Pacientes.Add(new Paciente
            {
                Nombre = Nombre,
                Apellido = Apellido,
                Edad = edad,
                Peso = peso,
                Estatura = estatura,
                SexoIndex = SexoIndex,
                ActividadIndex = ActividadIndex
            });

            // Limpiar
            Nombre = Apellido = Edad = Peso = Estatura = "";
            OnPropertyChanged(nameof(Nombre)); OnPropertyChanged(nameof(Apellido));
            OnPropertyChanged(nameof(Edad)); OnPropertyChanged(nameof(Peso));
            OnPropertyChanged(nameof(Estatura));
        }
    }

    void Eliminar(Paciente paciente)
    {
        if (Pacientes.Contains(paciente))
            Pacientes.Remove(paciente);
    }

    void Editar(Paciente paciente)
    {
        Nombre = paciente.Nombre;
        Apellido = paciente.Apellido;
        Edad = paciente.Edad.ToString();
        Peso = paciente.Peso.ToString();
        Estatura = paciente.Estatura.ToString();
        SexoIndex = paciente.SexoIndex;
        ActividadIndex = paciente.ActividadIndex;
        OnPropertyChanged(nameof(Nombre)); OnPropertyChanged(nameof(Apellido));
        OnPropertyChanged(nameof(Edad)); OnPropertyChanged(nameof(Peso));
        OnPropertyChanged(nameof(Estatura)); OnPropertyChanged(nameof(SexoIndex));
        OnPropertyChanged(nameof(ActividadIndex));

        Eliminar(paciente); // Se elimina para reemplazar con datos editados
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}