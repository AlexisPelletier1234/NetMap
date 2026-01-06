
# 🛰️ PortScan — Scanner de ports en C#
J'ai créé ce projet pour me pratiquer en Csharp, le but est de faire un mini clone de NMap
PortScan est un scanner de ports rapide, simple et extensible écrit en **C#**.  
Il permet d’identifier les ports ouverts, le service associé et d’extraire la **bannière** lorsque le service en renvoie une (SSH, FTP, SMTP, HTTP, Redis, etc.).

Ce projet a été créé pour l’apprentissage, l’expérimentation réseau et l’exploration des protocoles.

---

## 🚀 Fonctionnalités

- 🔍 **Scan rapide** des ports les plus courants  
- 🌐 **Scan complet** de 1 à 65535  
- 📡 **Détection de bannière** (SSH, FTP, SMTP, Redis, HTTP…)  
- 🧩 **Identification du service probable** selon le port  
- 🧪 **Validation IPv4** via Regex  
- 📊 **Progression en temps réel**  
- ❓ **Aide intégrée (`H`)**  

---

## 🧭 Utilisation

```bash
PortScan <option> <ip>
```

### Options

| Option | Description |
|--------|-------------|
| `R` | Scan rapide des ports importants |
| `C` | Scan complet (1 à 65535) |
| `H` | Affiche l’aide |

---

## 💡 Exemples

### Scan rapide
```bash
PortScan R 192.168.1.10
```

### Scan complet
```bash
PortScan C 10.0.0.5
```

### Aide
```bash
PortScan H
```

---

## 📡 Détection de bannière

Lorsque possible, PortScan lit la bannière envoyée par le service ou envoie une requête minimale (ex. une requête HTTP HEAD) pour provoquer une réponse.

Exemples de bannières détectées :

```
Port 22 Ouvert (SSH) → Bannière : SSH-2.0-OpenSSH_8.9p1
Port 25 Ouvert (SMTP) → Bannière : 220 smtp.example.com ESMTP
Port 80 Ouvert (HTTP) → Bannière : HTTP/1.1 400 Bad Request
Port 6379 Ouvert (Redis) → Bannière : -ERR wrong number of arguments...
```

---

## 🧱 Structure du projet

- `scanRapide()` → Scan des ports communs  
- `scanComplet()` → Scan 1 → 65535  
- `LireBanniere()` → Lecture de bannière  
- `Services` → Dictionnaire port → service  
- `ipValide()` → Validation IPv4  
- `aide()` → Documentation interne  
- `Main()` → Analyse des arguments & routage  

---

## ⚖️ Avertissement légal

Ce scanner est fourni pour un usage **éducatif** et pour analyser **vos propres systèmes**.  
Scanner des machines sans autorisation explicite peut être **illégal** selon votre pays.

L’auteur n’est pas responsable d’un usage abusif.

---

## 🛠️ Améliorations futures

- 🔥 Scan parallèle ultra rapide  
- 🌐 Scan UDP  
- 📦 Export JSON / CSV  
- 🎨 Sortie colorée + mode `--verbose`  
- 🧬 Analyse avancée des signatures/bannières  
- 🌍 Support IPv6  
- 🏷️ Scan personnalisé (range, timeout, ports spécifiques)  

---

## 👤 Auteur

Développé par **Alexis Pelletier**.  
Contributions et suggestions bienvenues !
