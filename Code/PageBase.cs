namespace EarWorm.Code
{
    public class PageBase : Microsoft.AspNetCore.Components.ComponentBase , System.IDisposable
    {
        public void Dispose() {
            OnPageUnload();
        }

        protected virtual void OnPageLoad() {

        }
        protected virtual void OnPageUnload() {
        }
    }
}
