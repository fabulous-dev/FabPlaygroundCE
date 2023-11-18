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

module Program =
    open type View
    
    [<EntryPoint>]
    let main(args) =
        let sharedContext = Context()
        
        let comp1 = Component(sharedContext, Counter())
        let comp2 = Component(sharedContext, Counter())
        let comp3 = Component(Counter())
        
        comp1.Widget.DataAChanged(null)
        let newCount1 = comp1.Widget.DataA
        comp2.Widget.DataAChanged(null)
        let newCount2 = comp2.Widget.DataA
        comp3.Widget.DataAChanged(null)
        let newCount3 = comp3.Widget.DataA
        0
        
