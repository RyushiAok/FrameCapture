[<AutoOpen>]
module Extensions
open System  
module Console =
    let progressBar (percent:float) =    
        Console.SetCursorPosition( 0, Console.CursorTop );
        let chars = Array.create<char> 82 ' '
        for i in 0..int(percent*80.0) do chars.[i+1] <- '#'
        chars.[0]  <- '['
        chars.[81] <- ']'
        printf "処理中... %s" (chars |> String.Concat)

module Double =
    let TryParse (x:string) =
        match Double.TryParse x with
        | true, v -> Some v
        | false, _ ->  None

module Int32 =
    let TryParse (x:string) = 
        match Int32.TryParse x with
        | true, v  -> Some v
        | false, _ -> None