

namespace IndoorCO2App_Android
{
    public partial class MainPage : ContentPage
    {
        private void OnEntryCompleted(object sender, EventArgs e)
        {
            var editor = sender as Editor;

            if (editor != null)
            {
                // Perform your logic when the Editor loses focus
                // For example, hide the keyboard if needed or validate the text.

                // Example: Hide the keyboard
                editor.Unfocus();  // This makes sure the editor loses focus and the cursor is removed.
            }
        }
    }
}
