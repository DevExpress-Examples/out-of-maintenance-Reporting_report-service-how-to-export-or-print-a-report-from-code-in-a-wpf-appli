using System.Printing;
using System.Windows.Controls;
using DevExpress.Mvvm.UI;

namespace T148944.Service {
    public class PrintDialogService : ServiceBase, IPrintDialogService {
        readonly PrintDialog dialog = new PrintDialog();

        public PrintQueue PrintQueue {
            get { return dialog.PrintQueue; }
        }

        public bool ShowDialog() {
            return dialog.ShowDialog() == true;
        }
    }
}
