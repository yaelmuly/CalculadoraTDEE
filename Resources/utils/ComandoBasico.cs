using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CalculadoraTDEE.Resources.utils
{
    public class ComandoBasico : ICommand
    {
        private readonly Action _ejecutar;
        private readonly Func<bool> _puedeEjecutar;

        public ComandoBasico(Action ejecutar, Func<bool> puedeEjecutar = null)
        {
            _ejecutar = ejecutar ?? throw new ArgumentNullException(nameof(ejecutar));
            _puedeEjecutar = puedeEjecutar;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => _puedeEjecutar == null || _puedeEjecutar();

        public void Execute(object parameter) => _ejecutar();

        public void NotificarCambioPuedeEjecutar()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
