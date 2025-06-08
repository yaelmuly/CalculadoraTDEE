using CalculadoraTDEE.model;
using System.Collections.ObjectModel;

namespace CalculadoraTDEE;

public partial class MainPage : ContentPage
{
    private ObservableCollection<Paciente> _pacientes;
    private Paciente _pacienteSeleccionado;
    private bool _modoEdicion = false;

    public MainPage()
    {
        InitializeComponent();
        _pacientes = new ObservableCollection<Paciente>();
        PacientesCollectionView.ItemsSource = _pacientes;
    }

    private async void OnNuevoPacienteClicked(object sender, EventArgs e)
    {
        _modoEdicion = false;
        _pacienteSeleccionado = null;
        LimpiarFormulario();
        MostrarFormulario();
    }

    private async void OnVerPacientesClicked(object sender, EventArgs e)
    {
        MostrarListaPacientes();
    }

    private async void OnCalcularClicked(object sender, EventArgs e)
    {
        LimpiarErrores();

        // Obtener valores de actividad
        var nivelesActividad = new double[] { 1.2, 1.375, 1.55, 1.725, 1.9 };
        var descripcionesActividad = new string[]
        {
            "Sedentario",
            "Ligero",
            "Moderado",
            "Intenso",
            "Muy intenso"
        };

        // Validar datos
        var validacion = Paciente.ValidarDatos(
            NombreEntry.Text?.Trim(),
            ApellidoEntry.Text?.Trim(),
            EdadEntry.Text?.Trim(),
            PesoEntry.Text?.Trim(),
            EstaturaEntry.Text?.Trim(),
            SexoPicker.SelectedIndex,
            ActividadPicker.SelectedIndex
        );

        if (!validacion.EsValido)
        {
            MostrarErrores(validacion.Errores);
            return;
        }

        try
        {
            // Crear o actualizar paciente
            if (_modoEdicion && _pacienteSeleccionado != null)
            {
                // Actualizar paciente existente
                _pacienteSeleccionado.Nombre = NombreEntry.Text.Trim();
                _pacienteSeleccionado.Apellido = ApellidoEntry.Text.Trim();
                _pacienteSeleccionado.Edad = int.Parse(EdadEntry.Text.Trim());
                _pacienteSeleccionado.Peso = double.Parse(PesoEntry.Text.Trim());
                _pacienteSeleccionado.Estatura = double.Parse(EstaturaEntry.Text.Trim());
                _pacienteSeleccionado.Sexo = SexoPicker.SelectedItem.ToString();
                _pacienteSeleccionado.NivelActividad = nivelesActividad[ActividadPicker.SelectedIndex];
                _pacienteSeleccionado.DescripcionActividad = descripcionesActividad[ActividadPicker.SelectedIndex];

                await DisplayAlert("Éxito", "Paciente actualizado correctamente", "OK");
            }
            else
            {
                // Crear nuevo paciente
                var nuevoPaciente = new Paciente
                {
                    Nombre = NombreEntry.Text.Trim(),
                    Apellido = ApellidoEntry.Text.Trim(),
                    Edad = int.Parse(EdadEntry.Text.Trim()),
                    Peso = double.Parse(PesoEntry.Text.Trim()),
                    Estatura = double.Parse(EstaturaEntry.Text.Trim()),
                    Sexo = SexoPicker.SelectedItem.ToString(),
                    NivelActividad = nivelesActividad[ActividadPicker.SelectedIndex],
                    DescripcionActividad = descripcionesActividad[ActividadPicker.SelectedIndex]
                };

                _pacientes.Add(nuevoPaciente);
                await DisplayAlert("Éxito", "Paciente agregado correctamente", "OK");
            }

            // Mostrar resultados
            await MostrarResultados(_modoEdicion ? _pacienteSeleccionado : _pacientes.Last());

            // Limpiar formulario
            LimpiarFormulario();
            OcultarFormulario();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al procesar los datos: {ex.Message}", "OK");
        }
    }

    private async void OnCancelarClicked(object sender, EventArgs e)
    {
        LimpiarFormulario();
        OcultarFormulario();
    }

    private async void OnVolverClicked(object sender, EventArgs e)
    {
        OcultarListaPacientes();
    }

    private async void OnPacienteSeleccionado(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Paciente paciente)
        {
            // Mostrar detalles del paciente
            await MostrarDetallesPaciente(paciente);

            // Deseleccionar
            PacientesCollectionView.SelectedItem = null;
        }
    }

    private async void OnEliminarPaciente(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is Paciente paciente)
        {
            bool confirmar = await DisplayAlert("Confirmar",
                $"¿Está seguro de eliminar a {paciente.NombreCompleto}?",
                "Sí", "No");

            if (confirmar)
            {
                _pacientes.Remove(paciente);
                await DisplayAlert("Éxito", "Paciente eliminado correctamente", "OK");
            }
        }
    }

    private async Task MostrarResultados(Paciente paciente)
    {
        var mensaje = $"RESULTADOS PARA {paciente.NombreCompleto.ToUpper()}\n\n" +
                     $"📊 ÍNDICE DE MASA CORPORAL (IMC)\n" +
                     $"IMC: {paciente.IMC:F1} kg/m²\n" +
                     $"Clasificación: {paciente.ClasificacionIMC}\n\n" +

                     $"🏃 PORCENTAJE DE GRASA CORPORAL\n" +
                     $"% Grasa: {paciente.PorcentajeGrasaCorporal:F1}%\n" +
                     $"Clasificación: {paciente.ClasificacionGrasaCorporal}\n\n" +

                     $"⚖️ PESO IDEAL\n" +
                     $"Peso ideal: {paciente.PesoIdeal:F1} kg\n" +
                     $"Diferencia: {(paciente.Peso - paciente.PesoIdeal):+0.1;-0.1;0} kg\n\n" +

                     $"🔥 GASTO ENERGÉTICO\n" +
                     $"BMR (metabolismo basal): {paciente.BMR:F0} cal/día\n" +
                     $"TDEE (gasto total): {paciente.TDEE:F0} cal/día\n" +
                     $"Actividad: {paciente.DescripcionActividad}";

        await DisplayAlert("Resultados del Cálculo", mensaje, "OK");
    }

    private async Task MostrarDetallesPaciente(Paciente paciente)
    {
        var acciones = new string[] { "Ver Resultados", "Editar", "Cancelar" };

        var accion = await DisplayActionSheet(
            $"Paciente: {paciente.NombreCompleto}",
            "Cancelar",
            null,
            acciones);

        switch (accion)
        {
            case "Ver Resultados":
                await MostrarResultados(paciente);
                break;
            case "Editar":
                EditarPaciente(paciente);
                break;
        }
    }

    private void EditarPaciente(Paciente paciente)
    {
        _modoEdicion = true;
        _pacienteSeleccionado = paciente;

        // Cargar datos en el formulario
        NombreEntry.Text = paciente.Nombre;
        ApellidoEntry.Text = paciente.Apellido;
        EdadEntry.Text = paciente.Edad.ToString();
        PesoEntry.Text = paciente.Peso.ToString();
        EstaturaEntry.Text = paciente.Estatura.ToString();

        SexoPicker.SelectedItem = paciente.Sexo;

        // Seleccionar nivel de actividad
        var nivelesActividad = new double[] { 1.2, 1.375, 1.55, 1.725, 1.9 };
        var indiceActividad = Array.IndexOf(nivelesActividad, paciente.NivelActividad);
        ActividadPicker.SelectedIndex = indiceActividad;

        MostrarFormulario();
        OcultarListaPacientes();
    }

    private void MostrarFormulario()
    {
        FormularioFrame.IsVisible = true;
        ListaPacientesFrame.IsVisible = false;
        NuevoPacienteBtn.IsVisible = false;
        VerPacientesBtn.IsVisible = false;
    }

    private void OcultarFormulario()
    {
        FormularioFrame.IsVisible = false;
        NuevoPacienteBtn.IsVisible = true;
        VerPacientesBtn.IsVisible = true;
    }

    private void MostrarListaPacientes()
    {
        ListaPacientesFrame.IsVisible = true;
        FormularioFrame.IsVisible = false;
        NuevoPacienteBtn.IsVisible = false;
        VerPacientesBtn.IsVisible = false;
    }

    private void OcultarListaPacientes()
    {
        ListaPacientesFrame.IsVisible = false;
        NuevoPacienteBtn.IsVisible = true;
        VerPacientesBtn.IsVisible = true;
    }

    private void LimpiarFormulario()
    {
        NombreEntry.Text = string.Empty;
        ApellidoEntry.Text = string.Empty;
        EdadEntry.Text = string.Empty;
        PesoEntry.Text = string.Empty;
        EstaturaEntry.Text = string.Empty;
        SexoPicker.SelectedIndex = -1;
        ActividadPicker.SelectedIndex = -1;
        LimpiarErrores();
    }

    private void LimpiarErrores()
    {
        NombreError.IsVisible = false;
        ApellidoError.IsVisible = false;
        EdadError.IsVisible = false;
        PesoError.IsVisible = false;
        EstaturaError.IsVisible = false;
        SexoError.IsVisible = false;
        ActividadError.IsVisible = false;
    }

    private void MostrarErrores(Dictionary<string, string> errores)
    {
        foreach (var error in errores)
        {
            switch (error.Key)
            {
                case "Nombre":
                    NombreError.Text = error.Value;
                    NombreError.IsVisible = true;
                    break;
                case "Apellido":
                    ApellidoError.Text = error.Value;
                    ApellidoError.IsVisible = true;
                    break;
                case "Edad":
                    EdadError.Text = error.Value;
                    EdadError.IsVisible = true;
                    break;
                case "Peso":
                    PesoError.Text = error.Value;
                    PesoError.IsVisible = true;
                    break;
                case "Estatura":
                    EstaturaError.Text = error.Value;
                    EstaturaError.IsVisible = true;
                    break;
                case "Sexo":
                    SexoError.Text = error.Value;
                    SexoError.IsVisible = true;
                    break;
                case "Actividad":
                    ActividadError.Text = error.Value;
                    ActividadError.IsVisible = true;
                    break;
            }
        }
    }
}