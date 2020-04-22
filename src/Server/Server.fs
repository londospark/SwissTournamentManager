open System.IO

open Giraffe
open Saturn

open Config

open SkiaSharp
open SkiaSharp.QrCode.Image
open System
open Thoth.Json.Net

let toOption (s: string): string option =
    if String.IsNullOrEmpty(s) then None
    else Some s

let tryGetEnv: string -> string option = System.Environment.GetEnvironmentVariable >> toOption

let publicPath = Path.GetFullPath "../Client/public"

let port =
    "SERVER_PORT"
    |> tryGetEnv |> Option.map uint16 |> Option.defaultValue 8085us

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
    forward "/players" Players.Controller.resource
    forward "/tournaments" Tournaments.Controller.resource
    forward "/players" Players.Controller.resource
    getf "/qrcode/%s" tournamentEntryQR
}

let webApp = router {
    forward "/api" apiApp
}


let extraCoders =
    Extra.empty
    |> Extra.withInt64

let app = application {
    url ("http://0.0.0.0:" + port.ToString() + "/")
    use_router webApp
    memory_cache
    use_static publicPath
    use_json_serializer(Thoth.Json.Giraffe.ThothSerializer(extra = extraCoders))
    use_gzip
    use_config (fun _ -> {connectionString = "DataSource=database.sqlite"} )
}

run app
