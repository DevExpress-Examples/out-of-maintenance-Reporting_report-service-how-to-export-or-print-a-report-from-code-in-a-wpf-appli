using System.Printing;

namespace T148944.Service {
    public interface IPrintDialogService {
        PrintQueue PrintQueue { get; }
        bool ShowDialog();
    }
}
