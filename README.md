# TestProject - Unity Projekt

## Opis Projekta

Testni projekt koji ima za cilj demonstrirati snalaženje i prilagođavanje projektu koji je potrebno sazdati u unity engineu.

Napominjem da sam svjestan mnogih dijelova ovog projekta koji bi se mogli unaprijediti ili optimizirati, da ne spominjemo estetsku odbojnost gotovo svakog elementa u mojih ruku djelu.

Ipak, s namjerom da se projekt dostavi na pregled u razumnom vremenskom roku, mislim da je ovo dobar ogledni primjerak.


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

### Struktura Foldera (Isključujući sistemske foldere)

- `Assets/Scenes/` - Sadrži sve scene projekta
- `Assets/Code/` - Ono što bi u javi bio folder koji nekako uspijeva probiti limit za dužinu filepatha u windowsima
  - `Assets/Code/Networking/` - U ovom slučaju, konektor za Picsum i pripadajuće modele
  - `Assets/Code/GameLogic/` - Razdvojen na controler za scenu i actor skripte za objekte u sceni
- `Assets/Code/Utils/` - Pomoćne skripte i alati
- `Assets/Enums/` - Jer volim kad mi compiler govori koje tagove stavljam
- `Assets/Resources/` - Slike kartica za memory
- `Assets/Prefabs/` - Sadrži prefabe korištene, razdvojene na sprite i ui inačice

*Napomena: U ovom slučaju sam razdvojio foldere kako bi meni osobno odgovaralo da se radi o projektu koji moram održavati sam, uzimajući u obzir njegovu veličinu. U timskom okruženju bi vjerojatno bilo uputnije glavnu podjelu napraviti po scenama ili kojem god načinu da se workload odluči razbiti po članovima tima. 

Na primjer, Asset folder bi mogao biti podjeljen na foldere po imenima scena, koji pak sadrže sve resuse koji su potrebni za tu konkretnu scenu, pa se od tamo vrši podjela na logiku, prefabe, spriteove i td.*

## Procjena Vremena Razvoja

Ako je vjerovati gitu, započeo sam ovaj projekt 4.12, te sam radio oko 5 sati dnevno na njemu do nedjelje. Uračunavajući još sat-dva posla dnevno, koliko sam već imao vremena za odvojiti, rekao bih da je vrijeme izvođenja bilo nekakvih 30-ak sati. 

Procijenio bih da je 10-ak sati od toga otišlo na prisjećanje API-ja i čitanje dokumentacije, ali teško mi je precizno odvojiti to vrijeme u procjeni.





