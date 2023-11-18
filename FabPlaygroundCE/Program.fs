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
                let! localState = state 10
                
                printfn $"Child evaluation. Count = {boundCount.Current} / LocalState = {localState.Current}"
                
                Widget(
                    DataA = boundCount.Current.ToString(),
                    DataB = localState.Current.ToString(),
                    DataAChanged = (fun _ -> boundCount.Set(boundCount.Current + 1)),
                    DataBChanged = (fun _ -> action()),
                    DataCChanged = fun _ -> localState.Set(localState.Current * 2)
                )
            }
            
        static member BindingParent() =
            view {
                let! count = state 0
                
                printfn $"Parent evaluation. Count = {count.Current}"
                
                Widget(
                    // NOTE: Recreating the Component here will lose the previous Context
                    DataA = Component(
                        View.BindingChild(Binding.ofState count, fun () ->
                            count.Set(count.Current * 10)
                        )
                    ),
                    DataAChanged = fun _ -> count.Set(count.Current + 1)
                )
                
            }

module Program =
    open type View
    
    [<EntryPoint>]
    let main(args) =
        let comp1 = Component(BindingParent())
        
        printfn "Trigger from child"
        let childWidget = (comp1.Widget.DataA :?> Component).Widget
        childWidget.DataAChanged()
        
        printfn "Trigger from child, local state only"
        let childWidget = (comp1.Widget.DataA :?> Component).Widget
        childWidget.DataCChanged()
        
        printfn "Trigger from child, but action from parent"
        let childWidget = (comp1.Widget.DataA :?> Component).Widget
        childWidget.DataBChanged()
        
        printfn "Trigger from parent"
        comp1.Widget.DataAChanged()
        0
        
