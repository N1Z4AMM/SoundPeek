# SoundPeek

## 1. Project Overview

Gua lagi bikin app buat windows desktop utility buat control volume per aplikasi tanpa harus masuk ke pengaturan dulu dengan sistem popup gitu pake shorcut ctrl + alt + v, 

## 2. Target Platfrom & Technology Stack

* **Minimum OS:** Windows 10
* **Target OS:** Windows 10 & 11
* **Tipe Aplikasi:** Windows aplikasi. Pake WPF, C# dan .NET8
* **Audio integration:** Windows Core Audio API
* **Icons:** Lucide Icons
* **Python:** Reserved for potential future audio-processing features

## 3. MPV Scope v0.1

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

## 4. UI/UX Guidelines

* Follow the existing Figma design as the primary visual reference.
* * Use a minimal, dark interface.
* Preserve the established spacing, layout, and component hierarchy.
* * Use Lucide icons with a 2px stroke.
* Avoid unnecessary gradients, excessive shadows, and decorative elements.
* Ensure sliders and buttons have clear interactive states.
* * Keep the popup compact and unobtrusive.
* The edge notch should expand on hover and collapse when appropriate.

## 5. Color Palette

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

### Iconography

- **Icon library:** Lucide Icons
- **Website:** https://lucide.dev/icons/
- **Stroke width:** 2 px
- **Style:** Outline, konsisten, dan minimalis.

## 6. Architecture Guidelines

* Keep UI code separate from audio-control logic.
* Use dedicated services for Windows audio integration.
* Avoid putting all application logic inside MainWindow.xaml.cs.
* Keep responsibilities small and clearly defined.
* Use asynchronous operations when appropriate.
* Handle unavailable audio sessions and disconnected output devices gracefully.

## 7. Coding Rules

* Write readable, maintainable C# code.
* tanya gua dulu sebelum membuat
* jangan buat fitur aneh
* pikir dulu sebelum membuat hal (hindari yang gak penting)
* Use clear and consistent naming.
* Avoid unnecessary abstractions and duplicated code.
* Do not use placeholder implementations for core audio functionality.
* Do not claim a feature works until it has been tested.
* Never silently swallow exceptions.
* Explain non-obvious Windows API behavior in comments.
* cek 2 kali hasil coding nya
* ikutin semua desain yang gua bikin, jangan ngawour desain nya
* gua gak suruh lu buat coding web downloader nya jadi jangan ganggu file SoundPeek di documents apa lagi ganggu cmd yang lagi jalanin Node.js nya
* cek apakah demo nya berhasil atau enggak
* commit hasilnya ke git dan push ke https://github.com/N1Z4AMM/SoundPeek dan ke branch dev 
* kasih tau gua apa yang lu bikin, apa ide yang lu pikir untuk selanjutnya, apa yang gagal
* kalau gua udah setuju dengan desain nya baru push ke branch main

## 8. Development Workflow

1. Before implementing a feature:
2. Inspect the existing project structure.
3. Understand the relevant code and dependencies.
4. Explain the proposed approach briefly.
5. Implement only the requested scope.
6. Build the application and resolve compilation errors.
7. Test the behavior when possible.
8. Summarize changed files, implementation details, and remaining issues.

## 9. Audio Requirements

* Use Windows Core Audio APIs for application audio sessions.
* Bind volume sliders to actual audio-session volume.
* Reflect external volume changes in the UI where supported.
* Handle applications starting, stopping, or recreating audio sessions.
* Distinguish between application-session volume and master output volume.
* Do not assume every running application has an active audio session.

## 10. Out of Scope

* Unless explicitly requested, do not implement:
* AI automation
* Cloud synchronization or user accounts
* Equalizer
* Noise reduction
* Automatic audio ducking
* Advanced audio routing
* Multi-output audio mixing
* Website or backend functionality

## 11. Final Principles

* Correct functionality comes before visual polish.
* Preserve the existing design and architecture.
* Prefer small, testable changes.
* Ask before making major architectural decisions.
* Clearly report anything that could not be implemented or tested.