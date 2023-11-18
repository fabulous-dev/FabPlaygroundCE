namespace FabPlaygroundCE

[<AutoOpen>]
module Components =
    type View with
        static member Counter() =
            view {
                let! count = state 0
                
                Widget(
                    DataA = count.Current.ToString(),
                    DataAChanged = fun _ -> count.Set(count.Current + 1)
                )
            }
            
        static member MVUCounter() =
            view {
                let! state = MVUCounter.program
                
                Widget(
                    DataA = state.State.Count.ToString(),
                    DataAChanged = (fun _ -> state.Dispatch(MVUCounter.Increment)),
                    DataBChanged = fun _ -> state.Dispatch(MVUCounter.Decrement)
                )
            }
            
        static member BindingChild(count: BindingRequest<'T>, action: unit -> unit) =
            view {
                let! boundCount = count
                
                Widget(
                    DataA = boundCount.Current.ToString(),
                    DataAChanged = (fun _ -> boundCount.Set(boundCount.Current + 1)),
                    DataBChanged = fun _ -> action()
                )
            }
            
        static member BindingParent() =
            view {
                let! count = state 0
                
                View.BindingChild(Binding.ofState count, fun () ->
                    count.Set(count.Current + 1)
                )
            }

module Program =
    open type View
    
    [<EntryPoint>]
    let main(args) =
        let comp1 = Component(BindingParent())
        printfn $"Data = {comp1.Widget.DataA}"
        comp1.Widget.DataAChanged(null)
        printfn $"Trigger from child: {comp1.Widget.DataA}"
        comp1.Widget.DataBChanged(null)
        printfn $"Trigger from parent: {comp1.Widget.DataA}"
        0
        
