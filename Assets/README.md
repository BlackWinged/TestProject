# TestProject - Unity Projekt

## Opis Projekta



## Verzija Unity

Projekt koristi verziju Unity editora 6000.2.14f1. 


## Preuzimanje i generalni opis strukture projekta

### Preuzimanje i pokretanje

1. **Preuzmite ili klonirajte projekt**
   ```bash
   git clone https://github.com/BlackWinged/TestProject.git
   cd TestProject
   ```

2. **Otvorite projekt u Unity Editoru**
   - Otvorite Unity Hub
   - Kliknite "Add" i odaberite `TestProject` folder
   - Unity će automatski prepoznati projekt i zatražiti instalaciju odgovarajuće verzije ako nije instalirana


3. **Pokrenite scenu**
   - Otvorite `Scenes/MainMenu.unity` kao početnu scenu
   - Pritisnite **Play** gumb u Unity Editoru (ili `Ctrl+P`)

### Scene i elementi projekta

- **MainMenu** (`Scenes/MainMenu.unity`) - Glavni odabirnik za ostatak scena
- **ImageGallery** (`Scenes/ImageGallery.unity`) - Galerija slika dohvaćenih s Picsum API-ja
- **VideoPlayer** (`Scenes/VideoPlayer.unity`) - Video player koji sam odlučio zakomplicirati više nego je bilo nužno potrebno
- **MemoryGame** (`Scenes/MemoryGame.unity`) - Odličan način za provjeriti u kojem stupnju demencije se korisnik trenutno nalazi

### Kontrole

- **Image gallery**: Povucite lijevom tipkom miša za pomicanje kamere, scroll za zoom
- **Memory Game**: Kliknite na kartice da ih okrenete, drag and drop za pomicanje kadra, potrebno ovisno o broju kartica na odabranoj težini
- **Video player**: WASD za kretanje, miš za gledanje. ESC gumb oslobađa kursor kako bi se mogle koristiti kontrole

## Picsum API

Projekt koristi **Picsum Photos API** (https://picsum.photos) za dohvaćanje nasumičnih slika s Unsplash-a. Konkretni api je odabran za ovaj projekt jer se pojavio između nulte i druge pozicije na gugl pretrazi za random image api.

### Implementacija u Projektu

Projekt koristi `PicsumConnector` klasu (`Code/Networking/PicsumConnector.cs`) za:

- **Asinkrono dohvaćanje** - `GrabRandomImagesAsync()` metoda koristi coroutine za neblokirajuće dohvaćanje
- **Sinkrono dohvaćanje** - `GrabRandomImages()` ovo je bio eksperiment i aktivno bricka webGL buildove, pa može poslužiti za testiranje debug vještina junior developera koji se zamjerio team leadu
- **Deserializacija** - Automatska konverzija JSON odgovora u `PicsumResponse` objekte. Ukoliko čitatelj bude zainteresiran za razgovor, živo me zanima zašto ne može hendlati gole array responseve
- **Error handling** - Integrirano rukovanje greškama kroz `Utils.LogErrorMessage()`

**Dokumentacija API-ja:** https://picsum.photos/
Iako je "dokumentacija" prilično jaka riječ u ovoj situaciji.

## Arhitektura Projekta

### Struktura Foldera


## Procjena Vremena Razvoja

Procjena vremena potrebnog za razvoj ovog projekta:

### Faze Razvoja



