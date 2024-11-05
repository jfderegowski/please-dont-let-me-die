using System;
using System.Threading.Tasks;

namespace NoReleaseDate.Common.Runtime.Extensions
{
    public static class ActionExtensions
    {
        private static async Task ExecuteInvokeAsync(Action action)
        {
            if (action == null) return;
            
            var task = new Task(action);
            
            task.Start(TaskScheduler.FromCurrentSynchronizationContext());
            
            task.Wait();
            
            await task;
        }

        public static async Task InvokeAsync(this Action action) =>
            await ExecuteInvokeAsync(action);

        public static async Task InvokeAsync<T>(this Action<T> action, T t) =>
            await ExecuteInvokeAsync(() => action(t));
        
        public static async Task InvokeAsync<T1, T2>(this Action<T1, T2> action, T1 t1, T2 t2) =>
            await ExecuteInvokeAsync(() => action(t1, t2));
        
        public static async Task InvokeAsync<T1, T2, T3>(this Action<T1, T2, T3> action, T1 t1, T2 t2, T3 t3) =>
            await ExecuteInvokeAsync(() => action(t1, t2, t3));
        
        public static async Task InvokeAsync<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action, T1 t1, T2 t2, T3 t3, T4 t4) =>
            await ExecuteInvokeAsync(() => action(t1, t2, t3, t4));
        
        public static async Task InvokeAsync<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5) =>
            await ExecuteInvokeAsync(() => action(t1, t2, t3, t4, t5));
        
        public static async Task InvokeAsync<T1, T2, T3, T4, T5, T6>(this Action<T1, T2, T3, T4, T5, T6> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6) =>
            await ExecuteInvokeAsync(() => action(t1, t2, t3, t4, t5, t6));
        
        public static async Task InvokeAsync<T1, T2, T3, T4, T5, T6, T7>(this Action<T1, T2, T3, T4, T5, T6, T7> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7) =>
            await ExecuteInvokeAsync(() => action(t1, t2, t3, t4, t5, t6, t7));
        
        public static async Task InvokeAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this Action<T1, T2, T3, T4, T5, T6, T7, T8> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8) =>
            await ExecuteInvokeAsync(() => action(t1, t2, t3, t4, t5, t6, t7, t8));
        
        public static async Task InvokeAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9) =>
            await ExecuteInvokeAsync(() => action(t1, t2, t3, t4, t5, t6, t7, t8, t9));

        public static async Task InvokeAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10) =>
            await ExecuteInvokeAsync(() => action(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10));

        public static async Task InvokeAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11) =>
            await ExecuteInvokeAsync(() => action(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11));

        public static async Task InvokeAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12) =>
            await ExecuteInvokeAsync(() => action(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12));
        
        public static async Task InvokeAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13) =>
            await ExecuteInvokeAsync(() => action(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13));
        
        public static async Task InvokeAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14) =>
            await ExecuteInvokeAsync(() => action(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14));
        
        public static async Task InvokeAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15) =>
            await ExecuteInvokeAsync(() => action(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15));

        public static async Task InvokeAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15, T16 t16) =>
            await ExecuteInvokeAsync(() => action(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16));
    }
}