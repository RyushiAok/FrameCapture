open System 
open System.Diagnostics
open System.IO
open OpenCvSharp
open OpenCvSharp.Extensions 
open Plotly.NET 

module Image =
    let canny (img:Mat) =
        let p = img.Canny(100.,100.)          
        p, p.CountNonZero()

let extractSlides (interval:float) (path:string) outpath threshold =  
    use videoCapture = new VideoCapture(path)
    let frameCnt = videoCapture.FrameCount
    let step = 
        let fps = videoCapture.Fps |> float
        if (interval < 1.0 / fps) 
        then 1
        else interval * fps |> int
    let mutable img = new Mat() 
    let mutable prevCountNonZero = 0   
    let mutable imgIdx = 0
    let ary = Array.zeroCreate<int> (frameCnt/step+1)
    for i in 0..step..frameCnt - 1 do 
        Console.progressBar ((float)i / (float)frameCnt)
        videoCapture.PosFrames <- int i 
        videoCapture.Read img |> ignore
        let gray,nonZeroCnt = Image.canny img 
        
        if abs (nonZeroCnt-prevCountNonZero) > threshold then
            BitmapConverter
                .ToBitmap(img)  // gray              
                .Save(sprintf @"%s/%d.jpg" outpath imgIdx) // i/step |> string
            imgIdx <- imgIdx + 1

        ary.[(i/step) |> int ] <- abs (nonZeroCnt-prevCountNonZero)
        prevCountNonZero <- nonZeroCnt     
        
    Console.progressBar 1.0 
    // // エッジ差分グラフ出力        
    // Chart.Histogram(ary)
    // |> Chart.withSize (800,500)
    // |> Chart.show 

[<EntryPoint>]
let main argv =    
    printfn ""
    printfn "------------------------------------------------------"
    printfn "                   Frame Capture                      "
    printfn "------------------------------------------------------"
    printfn ""
    printf  "Src Path (例: /app/tmp/src/test.mp4, ) \n> " 
    let path = Console.ReadLine() 
    match File.Exists path with 
    | false -> printfn "パスを確認してください。"
    | true ->
        let outpath = 
            sprintf @"%s/%s_%s" 
                // argv.[0]
                "/app/tmp/out"
                (Path.GetFileNameWithoutExtension path) 
                (DateTime.Now.ToString("yyyyMMddHmmss")) 
        Directory.CreateDirectory outpath |> ignore
        printf "\n間隔[s] (0以上) (ex: 5)\n> "   
        match Double.TryParse (Console.ReadLine()) with
        | Some interval when interval >= 0.0 ->
            printf "\n閾値 (ex: 2000)\n> "
            match Int32.TryParse (Console.ReadLine()) with 
            | None -> printfn "整数を入力してください。"
            | Some threshold ->     
                printfn ""
                extractSlides interval path outpath threshold
                printfn "\n\n成功！"
                printfn "\n出力先: %s\n" outpath 
                // Process.Start("Explorer.exe", outpath) |> ignore
        | _ -> printfn "0以上の数値を入力してください。"
    0