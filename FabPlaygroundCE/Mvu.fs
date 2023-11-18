namespace FabPlaygroundCE

open System.Runtime.CompilerServices

type MVU<'state, 'msg>(init: unit -> 'state, update: 'msg -> 'state -> 'state) =
    member this.Init = init
    member this.Update = update

[<Struct>]
type MVUState<'state, 'msg>(ctx: Context, update: 'msg -> 'state -> 'state, key: int, state: 'state) =
    member this.State = state
    member this.Dispatch(msg: 'msg) =
        let newState = update msg state
        ctx.SetValue(key, newState)
      
[<Extension>]
type MvuExtensions =
    [<Extension>]
    static member inline Bind(_: ViewBuilder, mvu: MVU<'state, 'msg>, [<InlineIfLambda>] continuation: MVUState<'state, 'msg> -> Contextual) =
        Contextual(fun ctx ->
            let key = ctx.MoveNext()
            
            let value =
                match ctx.Current with
                | ValueSome value -> unbox<'state> value
                | ValueNone ->
                    let initValue = mvu.Init()
                    ctx.SetCurrentValue(initValue)
                    initValue
            
            let state = MVUState(ctx, mvu.Update, key, value)
            (continuation state).Invoke(ctx)
        )
        
module MVUCounter =
    type State = { Count: int }
    type Msg = Increment | Decrement
    
    let init () = { Count = 0 }
    let update msg model =
        match msg with
        | Increment -> { Count = model.Count + 1 }
        | Decrement -> { Count = model.Count - 1 }
        
    let program = MVU(init, update)