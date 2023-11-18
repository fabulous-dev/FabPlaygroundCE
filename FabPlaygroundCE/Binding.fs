namespace FabPlaygroundCE

open System.Runtime.CompilerServices

(*

The idea of Binding is to listen to a State<'T> that is managed by another Context and be able to update it
while notifying the two Contexts involved (source and target)

let child (count: BindingRequest<int>) =
    view {
        let! boundCount = bind count
    
        Button($"Count is {boundCount.Value}", fun () -> boundCount.Set(boundCount.Value + 1))
    }
    
let parent =
    view {
        let! count = state 0
        
        VStack() {
            Text($"Count is {count.Value}")
            child (Binding.ofState count)
        }
    }

*)

type [<Struct>] BindingRequest<'T>(source: State<'T>) =
    member this.Source = source
    
module Binding =
    let inline ofState (state: State<'T>) =
        BindingRequest<'T>(state)

type [<Struct>] Binding<'T>(ctx: Context, source: State<'T>) =
    member this.Current = source.Current
    member this.Set(value: 'T) =
        source.Set(value)
        ctx.MarkAsDirty()

[<Extension>]
type BindingExtensions =
    [<Extension>]
    static member inline Bind(_: ViewBuilder, request: BindingRequest<'T>, [<InlineIfLambda>] continuation: Binding<'T> -> Contextual) =
        Contextual(fun ctx ->
            let state = Binding(ctx, request.Source)
            (continuation state).Invoke(ctx)
        )