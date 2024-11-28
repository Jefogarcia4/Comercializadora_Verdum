using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ComercializadoraVerdum
{
    public static class FormManager
    {
        // Contador de formularios abiertos
        private static int _openFormsCount = 0;

        // Incrementa el contador cuando se abre un formulario
        public static void IncrementOpenForms()
        {
            _openFormsCount++;
        }

        // Decrementa el contador cuando se cierra un formulario
        public static void DecrementOpenForms()
        {
            _openFormsCount--;
            // Si no hay más formularios abiertos, se cierra la aplicación
            if (_openFormsCount == 0)
            {
                Application.Exit();
            }
        }

        // Método para obtener el número de formularios abiertos
        public static int GetOpenFormsCount()
        {
            return _openFormsCount;
        }
    }

}
