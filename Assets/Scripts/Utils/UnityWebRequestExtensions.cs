using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine.Networking;

public static class UnityWebRequestExtensions
{
    public static TaskAwaiter<UnityWebRequest.Result> GetAwaiter(this UnityWebRequestAsyncOperation asyncOp)
    {
        var tcs = new TaskCompletionSource<UnityWebRequest.Result>();
        asyncOp.completed += obj => { tcs.SetResult(((UnityWebRequestAsyncOperation)obj).webRequest.result); };
        return tcs.Task.GetAwaiter();
    }
}