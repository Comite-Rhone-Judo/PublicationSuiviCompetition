// ========== Variables globales ==========

var gReloading;                 // Pour les gestion de l'autoreload
var gUseAutoReload = true;      // Pour activer ou desactiver l'autoreload
var gDelayAutoReloadSec = typeof gDelayAutoReloadSec !== 'undefined' ? gDelayAutoReloadSec : 60;   // Pour definir le delai de l'autoreload en secondes
var gDefaultAutoReload = typeof gDefaultAutoReload !== 'undefined' ? gDefaultAutoReload : false;    // Activation autoreload par defaut (si la variable est definie dans le script de la page)

window.onload = windowOnLoad;   // Gestionnaire d'evenements pour le chargement de la page par defaut

// ========== Gestion de la barre de navigation ==========
// Ouverture et fermeture de la barre de navigation
function openElement(elementName) {
    document.getElementById(elementName).style.display = "block";
}

function closeElement(elementName) {
    document.getElementById(elementName).style.display = "none";
}

// ========== Gestion de l'autoreload ==========
// A mettre sur le window.onload pour verifier automatiquement le reload
function checkReloading() {
    var timeoutms = (typeof gDelayAutoReloadSec === "undefined" || isNaN(gDelayAutoReloadSec)) ? 60000 : gDelayAutoReloadSec * 1000;

    // 1. Lire la préférence utilisateur dans la session (ex: "groupe_statistiques_site.html,autoReloadEnabled")
    var sessionPref = getInSession("autoReloadEnabled");
    var isEnabled = false;

    if (sessionPref !== null) {
        // L'utilisateur a déjà cliqué sur la case à cocher pour ce type de page, son choix prime
        isEnabled = (sessionPref === "true");
    } else {
        // Pas de choix mémorisé, on applique le paramètre par défaut issu du XSLT
        isEnabled = gDefaultAutoReload;

        // Rétrocompatibilité (au cas où l'utilisateur arrive depuis un vieux lien avec le hash)
        if (window.location.hash === "#autoreload") {
            isEnabled = true;
        }
    }

    // 2. Mettre à jour l'état visuel de la checkbox
    var cb = document.getElementById('cbActualiser');
    if (cb) {
        cb.checked = isEnabled;
    }

    // 3. Lancer le timer si le rechargement est actif
    // 3. Lancer le timer si le rechargement est actif
    if (isEnabled) {
        gReloading = setTimeout(function () {
            // --- DÉBUT MODIFICATION ANTI-CACHE ---
            // On sépare l'URL des paramètres (?) et du hash (#)
            var urlBase = window.location.href.split('?')[0].split('#')[0];
            var currentHash = window.location.hash; // Sauvegarde l'ancre éventuelle
            var timestamp = new Date().getTime(); // Génère le jeton unique

            // Redirection forcée (le .replace évite de remplir l'historique "Précédent" du navigateur)
            window.location.replace(urlBase + "?t=" + timestamp + currentHash);
            // --- FIN MODIFICATION ANTI-CACHE ---
        }, timeoutms);
    }
}

// Active/Désactive l'autoreload au clic sur la checkbox
function toggleAutoRefresh(cb) {
    var isEnabled = cb.checked;

    // 1. Mémoriser le choix de l'utilisateur pour cette page (écrase le paramètre par défaut)
    setInSession("autoReloadEnabled", isEnabled ? "true" : "false");

    // 2. Appliquer le comportement
    if (isEnabled) {
        // Optionnel : on maintient le hash pour indiquer visuellement dans l'URL qu'on s'actualise
        window.location.replace("#autoreload");

        // 1er refresh quasi-immédiat (comme dans votre ancien code)
        gReloading = setTimeout(function () { window.location.reload(); }, 100);
    } else {
        // On coupe le timer IMMÉDIATEMENT
        clearTimeout(gReloading);
        gReloading = null;

        // On nettoie l'URL (retrait du #) SANS provoquer de rechargement
        if (window.history && window.history.replaceState) {
            // Conserve l'URL exacte (avec le paramètre anti-cache ?t=...) mais retire le hash
            history.replaceState(null, null, window.location.pathname + window.location.search);
        } else {
            window.location.hash = ""; // Fallback pour très vieux navigateurs
        }
    }
}

// Restauration de l'état de la modale après un rechargement
function initModals() {
    var savedId = sessionStorage.getItem('tas_stat_active_judoka_id');

    if (savedId) {
        var rows = document.getElementsByClassName('tas-stat-clickable-row');
        for (var i = 0; i < rows.length; i++) {
            if (rows[i].getAttribute('data-id') === savedId) {
                // On rouvre la modale avec la ligne html correspondante
                openJudokaStatsModal(rows[i], true);
                break;
            }
        }
    }
}

// ========== Gestion du Thème Sombre ==========

// Vérifie et applique le thème au chargement
function checkDarkMode() {
    // Utilisation directe de sessionStorage pour que le choix soit global à toutes les pages
    var isDark = sessionStorage.getItem('tas_global_dark_mode') === 'true';

    // Met à jour la case à cocher si le menu est présent
    var cb = document.getElementById('cbDarkMode');
    if (cb) {
        cb.checked = isDark;
    }

    // Applique la classe sur le body
    if (isDark) {
        document.body.classList.add('dark-mode');
    } else {
        document.body.classList.remove('dark-mode');
    }
}

// Action au clic sur la case à cocher
function toggleDarkMode(cb) {
    var isDark = cb.checked;

    // Sauvegarde globale
    sessionStorage.setItem('tas_global_dark_mode', isDark ? 'true' : 'false');

    // Bascule visuelle immédiate
    if (isDark) {
        document.body.classList.add('dark-mode');
    } else {
        document.body.classList.remove('dark-mode');
    }
}

// ========== Gestion des evenements ==========

// Callback pour le chargement de la page
function windowOnLoad() {
    // Verifie la gestion de l'actualisation automatique (auto-reload)
    checkReloading();

    // Vérifie et applique le thème sombre
    checkDarkMode();

    // Charge les panels (categories, etc.)
    initPanels();

    // Les barres d'onglets
    initTabs();

    // Restaure si une modale etait ouverte
    initModals();
}

// ========== Gestion des onglets ==========

// Ouvre un onglet d'une barre d'onglets, tabGroupName est le nom du groupe (data-tabgroup) auquel appartient l'onglet
// Les onglets doivent avoir la classe tasTabType et les boutons de la barre d'onglets la classe tasTabBtnType
function openTab(tabGroupName, tabName, saveIt) {
    var i, tabs, btns, s, query;

    // Ferme tous les onglets de la barre d'onglets
    query = "div.tasTabType[data-tabgroup='" + tabGroupName + "']";
    tabs = document.querySelectorAll(query);
    for (i = 0; i < tabs.length; i++) {
        tabs[i].style.display = "none";
    }

    // RAZ les boutons de la barre d'onglets
    query = "button.tasTabBtnType[data-tabgroup='" + tabGroupName + "']";
    btns = document.querySelectorAll(query);
    for (i = 0; i < tabs.length; i++) {
        btns[i].className = btns[i].className.replace(" w3-indigo", "");
    }

    // Affiche l'onglet et le bouton correspondant
    s = 'btn' + tabName;
    for (i = 0; i < btns.length; i++) {
        if (btns[i].id == s) {
            btns[i].className += " w3-indigo";
        }

        if (tabs[i].id == tabName) {
            tabs[i].style.display = "block";
        }
    }

    if (saveIt) {
        setInSession(tabGroupName, tabName)
    }
}

// Initialise les onglets de la page, en ouvrant l'onglet qui est en session
function initTabs() {
    var tabs, v, grp;

    // Ferme tous les onglets de la barre d'onglets
    tabs = document.getElementsByClassName("tasTabType");
    for (i = 0; i < tabs.length; i++) {
        grp = tabs[i].dataset["tabgroup"];
        v = getInSession(grp);
        if (v == tabs[i].id) {
            openTab(grp, v, false)
        }
    }
}

// ========== Gestion des panneaux ==========

// Initialisation des panneaux au chargement de la page
function initPanels() {
    // Initialise les panneaux ouverts par défaut
    initPanelGroup("tasOpenedPanelType", "block");

    // Initialise les panneaux fermés par défaut
    initPanelGroup("tasClosedPanelType", "none");
}

// Applique l'état aux éléments d'une classe donnée (lecture session)
function initPanelGroup(className, defaultState) {
    let elements = document.getElementsByClassName(className);
    for (let i = 0; i < elements.length; i++) {
        let panelId = elements[i].id;
        let sessionState = getInSession(panelId);

        // Si une valeur existe en session on l'utilise, sinon on prend l'état par défaut
        let finalState = sessionState ? sessionState : defaultState;
        applyPanelState(panelId, finalState);
    }
}

// Fonction centrale pour appliquer visuellement l'état (Manipulation du DOM)
function applyPanelState(elementName, state) {
    let panel = document.getElementById(elementName);
    let expandIcon = document.getElementById(elementName + "Expand");
    let collapseIcon = document.getElementById(elementName + "Collapse");

    if (!panel) return;

    panel.style.display = state;

    if (state === "block") {
        if (collapseIcon) collapseIcon.style.display = "inline";
        if (expandIcon) expandIcon.style.display = "none";
    } else {
        if (collapseIcon) collapseIcon.style.display = "none";
        if (expandIcon) expandIcon.style.display = "inline";
    }
}

// Fonction appelée au clic sur les boutons d'accordéon
function togglePanel(elementName) {
    let panel = document.getElementById(elementName);
    if (!panel) return;

    let currentState = panel.style.display;
    let newState = (currentState === "none" || currentState === "") ? "block" : "none";

    // 1. Mise à jour visuelle
    applyPanelState(elementName, newState);

    // 2. Mémorisation de l'état dans le sessionStorage
    setInSession(elementName, newState);
}

// ========== Gestion de la session ==========

// enregistre une valeur dans la session
function setInSession(key, value) {
    let path = document.location.pathname;
    let fileName = path.substring(path.lastIndexOf('/') + 1);

    let fullKey = fileName + "," + key;

    sessionStorage.setItem(fullKey, value);
}

// Recupere une valeur dans la session
function getInSession(key) {
    let path = document.location.pathname;
    let fileName = path.substring(path.lastIndexOf('/') + 1);

    let fullKey = fileName + "," + key;

    return sessionStorage.getItem(fullKey);
}

// ========== Gestion de la modale des Statistiques ==========

/**
 * Formate une valeur technique (ex: "12.5") en affichage français (ex: "12,5").
 * Gère les valeurs nulles, indéfinies ou vides.
 */
function formatFr(valueStr, defaultValue = '0') {
    if (!valueStr || valueStr === '') return defaultValue;
    return valueStr.replace('.', ',');
}

/**
 * Raccourci sécurisé pour injecter du texte dans un élément du DOM.
 */
function setModalText(id, text) {
    let el = document.getElementById(id);
    if (el) el.innerText = text;
}

/**
 * Met à jour une barre de progression W3.CSS et son label associé.
 */
function setJauge(lblId, barId, valueStr) {
    let barElement = document.getElementById(barId);
    if (barElement) {
        // Le CSS exige la donnée technique avec le point
        barElement.style.width = (valueStr || '0') + '%';
    }

    // Le label visuel utilise la fonction utilitaire française
    setModalText(lblId, formatFr(valueStr) + ' %');
}

/**
 * Charge les données du judoka cliqué dans la modale et l'affiche.
 */
function openJudokaStatsModal(rowElement, skipAnimation) {
    // 1. En-tête de la modale
    setModalText('m-nom', rowElement.getAttribute('data-nom'));
    setModalText('m-cat', rowElement.getAttribute('data-cat'));
    setModalText('m-club', rowElement.getAttribute('data-club'));

    // 2. Résultats globaux
    setModalText('d-combats', rowElement.getAttribute('data-cbts') || '0');
    setModalText('d-tauxvic', formatFr(rowElement.getAttribute('data-vic')) + ' %');

    // 3. Profil des victoires
    setJauge('lbl-ippon', 'bar-ippon', rowElement.getAttribute('data-ippon'));
    setJauge('lbl-wazaawa', 'bar-wazaawa', rowElement.getAttribute('data-wazaawa'));
    setJauge('lbl-waza', 'bar-waza', rowElement.getAttribute('data-waza'));
    setJauge('lbl-yuko', 'bar-yuko', rowElement.getAttribute('data-yuko'));
    setJauge('lbl-shido3', 'bar-shido3', rowElement.getAttribute('data-shido3'));
    setJauge('lbl-hansoku', 'bar-hansoku', rowElement.getAttribute('data-hansoku'));
    setJauge('lbl-amf', 'bar-amf', rowElement.getAttribute('data-amf'));
    setJauge('lbl-decision', 'bar-decision', rowElement.getAttribute('data-decision'));

    // 4. Durées de combat
    setModalText('d-tmin', rowElement.getAttribute('data-tmin') || '-');
    setModalText('d-tmoy', rowElement.getAttribute('data-tmoy') || '-');
    setModalText('d-tmax', rowElement.getAttribute('data-tmax') || '-');

    // Mis de cote suite a manque de donnees TAS
    /*
    // 5. Golden Score
    let gsCbt = rowElement.getAttribute('data-gscbt') || '0';
    let gsPct = formatFr(rowElement.getAttribute('data-gspct'));
    setModalText('d-gscbt_pct', gsCbt + ' (' + gsPct + ' %)');
    setModalText('d-gsmoy', rowElement.getAttribute('data-gsmoy') || '-');
    */

    // 6. Discipline
    setModalText('d-pen', formatFr(rowElement.getAttribute('data-pen')));

    // --- NOUVEAU : Gestion de l'animation ---
    var modalContent = document.querySelector('#statsModal .w3-modal-content');
    if (modalContent) {
        if (skipAnimation) {
            modalContent.classList.remove('w3-animate-right'); // On supprime l'effet
        } else {
            // On s'assure que l'effet est bien là pour les clics manuels
            if (!modalContent.classList.contains('w3-animate-right')) {
                modalContent.classList.add('w3-animate-right');
            }
        }
    }
    // ----------------------------------------

    // Bloquer le défilement de <body> ET de <html> (Correction du double ascenseur)
    document.body.style.overflow = 'hidden';
    document.documentElement.style.overflow = 'hidden';

    // Afficher la modale
    document.getElementById('statsModal').style.display = 'block';

    // --- NOUVEAU : On mémorise le judoka actuellement consulté ---
    sessionStorage.setItem('tas_stat_active_judoka_id', rowElement.getAttribute('data-id'));
}

/**
 * Ferme la modale
 */
function closeJudokaStatsModal() {
    // Masquer la modale
    document.getElementById('statsModal').style.display = 'none';

    // Restaurer le défilement normal de la page
    document.body.style.overflow = '';
    document.documentElement.style.overflow = '';

    // --- NOUVEAU : On efface la mémoire à la fermeture ---
    sessionStorage.removeItem('tas_stat_active_judoka_id');
}