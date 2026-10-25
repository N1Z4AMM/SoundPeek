# SoundPeek — Product Requirements Document

> **A small mixer. More control.**

---

## 1. Problem Statement

Mengatur volume aplikasi di Windows sering mengharuskan pengguna membuka Volume Mixer dan mencari aplikasi yang ingin diubah. SoundPeek membuat kontrol tersebut lebih cepat diakses melalui panel kecil yang dapat dibuka kapan pun tanpa mengganggu alur kerja.

SoundPeek dirancang sebagai utilitas audio desktop yang ringan, minimalis, dan mudah diakses melalui notch di tepi layar yang terbuka saat pengguna mengarahkan kursor (*hover*), dengan global shortcut sebagai alternatif.

## 2. Target Users

- Pengguna Windows yang sering menjalankan banyak aplikasi sekaligus.
- Pengguna yang sering berpindah antara musik, browser, game, dan voice chat.
- Pengguna yang menyukai utilitas desktop kecil dan tidak mengganggu alur kerja.
- Gamer yang membutuhkan akses cepat untuk mengatur volume setiap aplikasi, terutama pengguna Windows 10 yang merasa pengaturan volume bawaan kurang praktis.

## 3. Product Goals

1. Mempercepat akses ke kontrol volume aplikasi.
2. Menyediakan kontrol volume per aplikasi melalui satu panel ringkas.
3. Memberikan pengalaman desktop yang minimalis dan tidak mengganggu.
4. Berjalan secara lokal tanpa akun pengguna maupun backend online.
5. Menjadi fondasi untuk fitur audio lanjutan pada versi berikutnya.

## 4. Feature Priorities

### 4.1 MVP — Version 1.0

| Feature | Priority |
|---|---|
| Daftar aplikasi dengan sesi audio aktif | Must-have |
| Slider volume untuk setiap aplikasi | Must-have |
| Mute/unmute per aplikasi | Must-have |
| Kontrol volume perangkat output utama | Must-have |
| Popup App dengan shorcut dan animasi | Must-have |
| Global shortcut untuk membuka panel | Must-have |
| System tray dan menu keluar | Must-have |
| Pengaturan dasar aplikasi | Must-have |
| Preset volume untuk aplikasi | Must-have |
| Preset volume berdasarkan perangkat output | Must-have |
| Manajemen perangkat output default | Must-have |
| Penyimpanan preferensi volume aplikasi | Perlu validasi teknis |

### 4.2 Next — Version 2.0 dan seterusnya

| Feature | Priority |
|---|---|
| Tema, transparansi, dan ukuran panel | Planned |
| Noise reduction berbasis Python | Planned |
| Auto volume ducking saat seseorang berbicara di Discord | Planned |
| Equalizer | Planned |
| Audio routing | Planned |
| Dukungan aplikasi UWP/Microsoft Store | Planned |
| WASAPI untuk kebutuhan pemrosesan audio lanjutan | Planned |
| Audio router multi-output | Planned |
| AI automation berbasis aturan | Planned |

### 4.3 Optional Features

| Feature | Priority |
|---|---|
| Visualisasi audio real-time | Optional |
| Pemetaan global hotkey yang dapat dikustomisasi | Optional |
| Widget desktop | Optional |
| Preset berdasarkan skenario: Game, Music, Film, Work | Optional |

**Catatan:** Preset aplikasi dan preset perangkat output merupakan target MVP. Preset skenario penggunaan merupakan fitur opsional yang berbeda.

### 4.4 Non-Goals

Hal-hal berikut tidak menjadi target pengembangan awal:

- Akun pengguna.
- Cloud synchronization.
- Backend online.
- AI automation pada MVP.
- Sistem distribusi atau pemrosesan audio kompleks sebelum kontrol volume dasar stabil.

SoundPeek berfokus pada utilitas lokal yang ringan dan praktis.

---

## 5. Interaction Design

### 5.1 Edge Notch

Notch berfungsi sebagai titik akses utama SoundPeek.

- Berada di tepi layar dan mudah ditemukan.
- Membuka panel mixer saat kursor diarahkan ke notch.
- Menutup atau menyusut kembali ketika tidak digunakan.
- Tidak mengganggu penggunaan aplikasi lain.
- Memiliki global shortcut sebagai metode akses alternatif.

Perilaku ketika aplikasi berada dalam mode fullscreen, ketika beberapa monitor digunakan, serta ketika kursor meninggalkan panel perlu diuji.

### 5.2 Quick Mixer

Panel mixer menyediakan:

- Daftar aplikasi yang memiliki sesi audio.
- Nama atau identitas visual aplikasi.
- Slider volume individual.
- Tombol mute/unmute.
- Kontrol volume perangkat output.
- Akses ke pengaturan dan system tray.

### 5.3 System Tray

System tray menyediakan akses ke fungsi dasar aplikasi, termasuk membuka mixer, membuka pengaturan, dan keluar sepenuhnya dari aplikasi.

---

## 6. Success Criteria

MVP dianggap memenuhi tujuan awal apabila:

- Panel dapat dibuka melalui hover dan global shortcut.
- Perubahan volume diterapkan pada sesi audio aplikasi yang benar.
- Mute/unmute bekerja sesuai sesi audio yang dipilih.
- Volume perangkat output dapat dikontrol dengan benar.
- Panel tetap responsif ketika beberapa aplikasi memiliki sesi audio.
- Pengaturan yang memang dirancang untuk disimpan tetap tersedia setelah aplikasi dibuka kembali.
- Aplikasi dapat ditutup sepenuhnya melalui menu keluar.
- Penggunaan CPU dan RAM tetap wajar saat aplikasi idle maupun saat mixer digunakan.

## 7. Technical Considerations

Windows menyediakan sistem sesi audio melalui Core Audio API. Namun, tidak semua aplikasi selalu memiliki sesi audio aktif, dan perilaku sesi dapat berbeda antara aplikasi.

Hal yang perlu divalidasi:

- Pendeteksian dan perubahan volume sesi audio.
- Perilaku ketika aplikasi mulai atau berhenti memutar audio.
- Perbedaan perilaku pada Windows 10 dan Windows 11.
- Pemilihan perangkat output dan perubahan perangkat default.
- Penyimpanan serta penerapan kembali preferensi volume.
- Perilaku hover, fokus jendela, dan posisi notch.
- Penggunaan CPU, RAM, dan responsivitas saat idle.

Fitur lanjutan seperti noise reduction, equalizer, dan audio routing membutuhkan evaluasi arsitektur tersendiri sebelum implementasi.

---

## 8. Technology Stack

### 8.1 Desktop Application

- **Language:** C#
- **UI Framework:** WPF
- **Runtime:** .NET 8
- **Audio API:** Windows Core Audio API
- **Audio Library:** Dievaluasi sesuai kebutuhan implementasi
- **Future Audio Processing:** Python

Python direncanakan untuk fitur pemrosesan audio lanjutan, terutama noise reduction. Integrasinya dengan aplikasi C# akan ditentukan saat fitur tersebut mulai dikembangkan.

### 8.2 Website

- **Framework:** React
- **Language:** TypeScript
- **Styling:** Tailwind CSS 4
- **Runtime / Tooling:** Node.js

Website digunakan untuk memperkenalkan SoundPeek, menyediakan informasi versi, dan menjadi titik akses untuk unduhan aplikasi.

---

## 9. Design System

### 9.1 Color Palette

| Token | Hex | Usage |
|---|---|---|
| Background | `#080808` | Latar utama |
| Surface | `#121212` | Panel dan permukaan utama |
| Surface Elevated | `#1C1C1C` | Elemen yang berada di atas surface |
| Border | `#292929` | Outline dan pemisah |
| Accent | `#676AF0` | Warna aksen utama |
| Icon | `#929292` | Ikon default |
| Icon Hover | `#EDEDED` | Ikon saat hover |
| Hover Active | `#676AF0` | Elemen interaktif aktif |
| Icon Disabled | `#505050` | Ikon nonaktif |

### 9.2 Iconography

- **Icon library:** Lucide Icons
- **Website:** https://lucide.dev/icons/
- **Stroke width:** 2 px
- **Style:** Outline, konsisten, dan minimalis.

Warna aksen digunakan secara selektif pada kontrol aktif, slider, indikator, dan tombol utama.

---

## 10. Release Strategy

### Version 1.0 — Core Mixer

Fokus pada notch, kontrol volume per aplikasi, volume output, system tray, shortcut, dan pengaturan dasar.

### Version 2.0 — Audio Enhancement

Fokus pada fitur pemrosesan audio, termasuk noise reduction dan auto volume ducking, setelah kelayakan teknis serta performanya divalidasi.

### Future Versions

Pengembangan equalizer, audio routing, visualisasi, widget, dan otomatisasi dilakukan berdasarkan hasil pengujian serta kebutuhan pengguna.

---

## 11. Project Ownership

**Product:** SoundPeek  
**Production by:** DevN  
**Tagline:** A small mixer. More control.
