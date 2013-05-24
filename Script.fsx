
#load @"C:\Users\Suzanna Kovoor\Desktop\Neumio\Code\Common\FSharpChart.fsx"
#load "AdaBooster.fs"

open AdaBooster
open MSDN.FSharp.Charting
open MSDN.FSharp.Charting.ChartTypes
open System
open System.Drawing
open System.Windows.Forms
open System.Windows.Forms.DataVisualization.Charting

let trainingData = [|(box (9., 6.), AdaBooster.ClassLabel.POS); (box (7., 8.), AdaBooster.ClassLabel.POS); (box (5., 6.3), AdaBooster.ClassLabel.POS); (box (6., 6.), AdaBooster.ClassLabel.POS); (box (7., 10.2), AdaBooster.ClassLabel.POS); (box (9., 9.), AdaBooster.ClassLabel.POS); (box (8., 7.), AdaBooster.ClassLabel.POS); (box (8., 9.), AdaBooster.ClassLabel.POS); (box (7., 9.), AdaBooster.ClassLabel.POS); (box (9., 9.), AdaBooster.ClassLabel.POS); (box (6., 8.), AdaBooster.ClassLabel.POS); (box (6., 7.), AdaBooster.ClassLabel.POS); (box (4., 5.3), AdaBooster.ClassLabel.POS);
                    (box (2., 6.), AdaBooster.ClassLabel.NEG); (box (9., 13.), AdaBooster.ClassLabel.NEG); (box (4., 1.), AdaBooster.ClassLabel.NEG); (box (1., 13.), AdaBooster.ClassLabel.NEG); (box (6., 11.), AdaBooster.ClassLabel.NEG); (box (10., 11.), AdaBooster.ClassLabel.NEG); (box (3., 5.), AdaBooster.ClassLabel.NEG); (box (1., 13.), AdaBooster.ClassLabel.NEG); (box (6., 3.), AdaBooster.ClassLabel.NEG); (box (10., 3.), AdaBooster.ClassLabel.NEG); (box (1., 4.), AdaBooster.ClassLabel.NEG); (box (1., 8.), AdaBooster.ClassLabel.NEG); (box (2., 6.), AdaBooster.ClassLabel.NEG);
                    (box (12., 2.), AdaBooster.ClassLabel.NEG); (box (12., 6.), AdaBooster.ClassLabel.NEG);(box (12., 8.6), AdaBooster.ClassLabel.NEG);(box (12., 10.), AdaBooster.ClassLabel.NEG);
                    (box (14., 2.), AdaBooster.ClassLabel.NEG); (box (14., 6.), AdaBooster.ClassLabel.NEG);(box (14., 8.6), AdaBooster.ClassLabel.NEG);(box (14., 10.), AdaBooster.ClassLabel.NEG);|]


let weakClassifierXLess  threshold (point : obj)  =
    let (x : float, y : float) = unbox (point)
    match x with
        | _ when x <= threshold -> ClassLabel.POS
        | _ -> ClassLabel.NEG
let weakClassifierYLess  threshold (point : obj)  =
    let (x : float, y : float) = unbox (point)
    match y with
        | _ when y <= threshold -> ClassLabel.POS
        | _ -> ClassLabel.NEG
let weakClassifierXGreater  threshold (point : obj)  =
    let (x : float, y : float) = unbox (point)
    match x with
        | _ when x > threshold -> ClassLabel.POS
        | _ -> ClassLabel.NEG
let weakClassifierYGreater  threshold (point : obj)  =
    let (x : float, y : float) = unbox (point)
    match y with
        | _ when y > threshold -> ClassLabel.POS
        | _ -> ClassLabel.NEG

//generate weak classifiers
let wkXLessThan : (obj -> ClassLabel)  list  = 
    [0.0 .. 1.0 .. 14.0]
    |> List.map( fun threshold -> weakClassifierXLess threshold )

let wkYLessThan  : (obj -> ClassLabel)  list    = 
    [0.0 .. 1.0 .. 14.0]
    |> List.map( fun threshold -> weakClassifierYLess threshold )

let wkXGreaterThan : (obj -> ClassLabel)  list  = 
    [0.0 .. 1.0 .. 14.0]
    |> List.map( fun threshold -> weakClassifierXGreater threshold )

let wkYGreaterThan : (obj -> ClassLabel)  list  = 
    [0.0 .. 1.0 .. 14.0]
    |> List.map( fun threshold -> weakClassifierYGreater threshold )

let weakClassifiers   =  List.append wkXGreaterThan wkYGreaterThan  
                         |> List.append wkYLessThan 
                         |> List.append wkXLessThan
                         |> List.toArray 
                                            
let adaBoosterClassifier = new AdaBooster( trainingData, weakClassifiers, 10)

let  strongClassifier = adaBoosterClassifier.Train 


strongClassifier
adaBoosterClassifier.WeightMatrix
adaBoosterClassifier.ScoutingMatrix


let dat1= 
    let mutable d = []
    for i in [0.0 .. 0.5 .. 14.0] do
        for j in [0.0 .. 0.5 .. 14.0] do
            let (x,y) = adaBoosterClassifier.Predict (box (i, j))
            if  y = ClassLabel.POS then
                d <-(i,j) ::d
    d




let (datapos:(float*float)seq) = trainingData
                                |> Array.filter (fun (x,y) -> y = AdaBooster.ClassLabel.POS)
                                |> Array.map(fun (x,y) -> unbox(x) )
                                |> Array.toSeq  
let (dataneg:(float*float)seq) = trainingData
                                |> Array.filter (fun (x,y) -> y = AdaBooster.ClassLabel.NEG)
                                |> Array.map(fun (x,y) -> unbox(x) )
                                |> Array.toSeq  

[
    FSharpChart.Point[ for (x,y) in datapos -> x, y ]  |> FSharpChart.WithSeries.Marker(Color.Blue):> ChartTypes.GenericChart
    FSharpChart.Point[ for (x,y) in dataneg -> x, y ]  |> FSharpChart.WithSeries.Marker(Color.Red):> ChartTypes.GenericChart
    FSharpChart.Point[ for (x,y) in dat1 -> x, y ]  |> FSharpChart.WithSeries.Marker(Color.Black, Style = MarkerStyle.Star6):> ChartTypes.GenericChart    
    ]

|> FSharpChart.Combine
|> FSharpChart.WithArea.AxisY
        ( Minimum = -1.0, Maximum = 16.0, 
          MajorGrid = Grid(LineColor = Color.LightGray) )


