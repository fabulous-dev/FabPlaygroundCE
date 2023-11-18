namespace FabPlaygroundCE

open System.Runtime.CompilerServices

type StateRequest<'T> = delegate of unit -> 'T

type [<Struct>] State<'T>(ctx: Context, key: int, value: 'T) =
    member this.Current = value
    member this.Set(value: 'T) = ctx.SetValue(key, value)

[<Extension>]
type StateExtensions =
    [<Extension>]
    static member inline Bind(_: ViewBuilder, [<InlineIfLambda>] fn: StateRequest<'T>, [<InlineIfLambda>] continuation: State<'T> -> Contextual) =
        Contextual(fun ctx ->
            let key = ctx.MoveNext()
            
            let value =
                match ctx.Current with
                | ValueSome value -> unbox<'T> value
                | ValueNone ->
                    let value = fn.Invoke()
                    ctx.SetCurrentValue(value)
                    value
                    
            let state = State(ctx, key, value)
            (continuation state).Invoke(ctx)
        )
        
[<AutoOpen>]
module StateHelpers =
    let inline state value = StateRequest(fun () -> value)