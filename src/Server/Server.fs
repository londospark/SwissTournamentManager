open System.IO
open System.Threading.Tasks

open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open FSharp.Control.Tasks.V2
open Giraffe
open Saturn
open Shared

open Config

open SkiaSharp
open SkiaSharp.QrCode.Image
open System

let tryGetEnv = System.Environment.GetEnvironmentVariable >> function null | "" -> None | x -> Some x

let publicPath = Path.GetFullPath "../Client/public"

let port =
    "SERVER_PORT"
    |> tryGetEnv |> Option.map uint16 |> Option.defaultValue 8085us

let tournaments : Tournament list =
    [
        {Name = "BCS London Cardfight Vanguard Premium"; Code = "CFV" }
        {Name = "BCS London Buddyfight"; Code = "FCB" }
    ]

let findTournament (code: string) (tournaments: Tournament list) : Tournament option =
    tournaments
    |> List.filter (fun item -> item.Code = code)
    |> List.tryHead

let tournamentController = controller {
    index (fun ctx -> tournaments |> Controller.json ctx)
    show (fun ctx (key: string) -> tournaments |> findTournament key |> Controller.json ctx)
}

let image (x : MemoryStream) : HttpHandler =
    setHttpHeader "Content-Type" "image/png"
    >=> setBody (x.ToArray())

let generateQRCode (host: string) (tournamentcode: string) : MemoryStream =
    let url = host + "/#Enter/" + tournamentcode
    let qrCode = QrCode(url, Vector2Slim(256, 256), SKEncodedImageFormat.Png)
    use stream = new MemoryStream()
    qrCode.GenerateImage(stream)
    stream

let tournamentEntryQR code = image (generateQRCode "http://localhost:8080" code)

let apiApp = router {
    get "/init" (fun next ctx ->
        task {
            let model : ApplicationState = { Tournaments = [] }
            return! json model next ctx
        })
    forward "/tournaments" Tournaments.Controller.resource
    //forward "/tournaments" tournamentController
    getf "/qrcode/%s" tournamentEntryQR
}

let webApp = router {
    forward "/api" apiApp
}

let app = application {
    url ("http://0.0.0.0:" + port.ToString() + "/")
    use_router webApp
    memory_cache
    use_static publicPath
    use_json_serializer(Thoth.Json.Giraffe.ThothSerializer())
    use_gzip
    use_config (fun _ -> {connectionString = "DataSource=database.sqlite"} )
}

run app
