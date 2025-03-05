

namespace IndoorCO2App_Multiplatform
{
    public partial class MainPage : ContentPage
    {
        private void OnEntryCompleted(object sender, EventArgs e)
        {

            if(sender is VisualElement input)
            {
                input.Unfocus();
            };
            RecoveryData.customNotes = NotesEditor.Text;
        }
    }
}
