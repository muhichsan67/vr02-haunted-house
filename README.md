# Handoff: World Space UI (Objective Board + Floating Prompt)

Status: selesai dibuat dan sudah jadi prefab. Siap dipasang ke scene asli.

Lokasi prefab: `_Project/Prefabs/UI/`
- `WorldUI_ObjectiveBoard.prefab`
- `WorldUI_FloatingPrompt.prefab`

Dites di scene: `_Project/Scenes/Scene_WorldUITest`

---

## WorldUI_ObjectiveBoard

Fungsi: papan statis nempel di dinding, nampilin progress checklist (contoh isi: "PAPER TERKUMPUL: 0/4").

Struktur:
```
WorldUI_ObjectiveBoard (Canvas, World Space)
└── Content
    ├── Panel_BoardBG (Image, background gelap)
    └── TMP_ObjectiveText (TextMeshPro)
```

Cara pasang:
1. Drag prefab ke scene asli, tempel di depan dinding/permukaan yang dipilih Orang 4.
2. Geser sedikit menjauh dari permukaan dinding (kira-kira 0.1 unit) biar nggak clipping/tembus.
3. Cek Rotation Y — kalau teksnya kebalik/mirror pas dilihat dari arah pemain, ubah Rotation Y jadi 180.
4. Scale Canvas sudah diset kecil (0.002 di X/Y/Z) supaya ukuran board masuk akal di dunia 3D — biasanya nggak perlu diubah lagi, kecuali dinding aslinya jauh lebih besar/kecil dari cube dummy yang dipakai testing.

Yang perlu diisi/diubah pihak lain:
- Isi teks di `TMP_ObjectiveText` disesuaikan sama sistem counting objective asli (kalau ada script counter, tinggal ganti teksnya via script, bukan manual).
- Posisi & rotasi final tergantung bentuk dinding asli dari Orang 4.

---

## WorldUI_FloatingPrompt

Fungsi: teks interaksi yang muncul/hilang otomatis (contoh isi: "Tekan E untuk Mengambil Kertas"). Beda dari Objective Board — ini reusable, bukan cuma satu instance statis.

Struktur:
```
WorldUI_FloatingPrompt (Canvas, World Space)
└── Content
    └── TMP_PromptText (TextMeshPro, outline aktif)
```

Cara pasang (buat Orang 3, raycast detector):
1. Drag prefab ini jadi **child** dari tiap objek interaktif (Paper, Pintu, dll) — bukan objek berdiri sendiri di scene.
2. Posisi relatif terhadap parent-nya diatur manual sesuai bentuk objek (misal di atas Paper, atau di depan Pintu setinggi mata).
3. Logic show/hide belum ada di prefab ini — itu bagian Orang 3, kemungkinan lewat:
   - Toggle `SetActive(true/false)` di GameObject `WorldUI_FloatingPrompt`, atau
   - Toggle komponen `CanvasGroup` (alpha 0/1) kalau mau ada fade.
4. Teks di `TMP_PromptText` perlu diganti sesuai objeknya masing-masing (isi placeholder sekarang: "Tekan E untuk Mengambil Kertas" — ganti manual per instance, misal jadi "Tekan E untuk Membuka Pintu" buat prefab yang dipasang di Pintu).
5. Cek Rotation Y sama seperti Objective Board — kalau kebalik, set ke 180.

Catatan penting: karena ini di-drag berkali-kali (satu per objek interaktif), pastikan tiap instance di-**unpack** kalau mau ubah teks per-objek tanpa ngubah prefab aslinya (klik kanan instance → Prefab → Unpack, atau edit langsung di instance level asal jangan apply ke prefab source).

---

## Kontak kalau ada masalah
- Salah warna/ukuran/posisi visual → tanya Markus (pembuat prefab ini)
- Logic show/hide raycast → tanggung jawab Orang 3
- Bentuk dinding/objek fisik → tanggung jawab Orang 4
