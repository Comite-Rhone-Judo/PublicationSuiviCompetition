(function () {
    // --- Configuration et Etat ---
    var config = {
        dureeRotation: 10,      // Valeur par défaut
        combatsParPage: 5,      // Valeur par défaut
        layoutMode: 4,           // Valeur par défaut
        urlRedirecteur: '',     // Valeur par defaut
        autoAjustementTexte: true, // Par défaut activé

        // Constantes pour la taille minimum des textes (en pixels)
        minFontSizeNom: 14,
        minFontSizeClub: 12,
        minFontSizeCatTitre: 12, // Limite basse pour le nom de la catégorie
        minFontSizeCatSub: 10    // Limite basse pour le niveau/tour
    };

    var state = {
        currentTapisGroupIndex: 1,    // Page de tapis courante (1, 2, ...)
        maxTapisGroups: 1,            // Nombre total de pages de tapis
        currentCombatPage: 1,         // Page de combat interne courante (1, 2, ...)
        maxCombatPagesCurrentView: 1, // Max pages de combats parmi les tapis affichés
        timer: null,
        progressBar: null
    };

    // --- Template Dédié : Générateur de puces de pagination ---
    function generatePaginationDots(currentPage, totalPages) {
        var html = '';
        for (var i = 1; i <= totalPages; i++) {
            if (i === currentPage) {
                // Puce active gérée par les classes CSS (w3-dot-active pour l'animation breathe)
                html += '<span class="w3-text-orange page-dot page-dot-active w3-dot-active">&#9679;</span>';
            } else {
                // Puce inactive gérée par les classes CSS
                html += '<span class="w3-text-white page-dot page-dot-inactive">&#9679;</span>';
            }
        }
        return html;
    }

    // --- Initialisation ---
    function init() {
        var container = document.getElementById('main-container');
        state.progressBar = document.getElementById('progress-bar');

        if (container) {
            config.dureeRotation = parseInt(container.getAttribute('data-duree-rotation')) || 10;
            config.combatsParPage = parseInt(container.getAttribute('data-combats-par-page')) || 8;
            config.layoutMode = parseInt(container.getAttribute('data-layout-mode')) || 4;
            config.urlRedirecteur = container.getAttribute('data-url-redirecteur') || '';
            var autoAjustAttr = container.getAttribute('data-auto-ajustement');
            config.autoAjustementTexte = (autoAjustAttr !== 'false');
        }

        var allTapis = document.querySelectorAll('.tapis-card');
        var maxPage = 0;
        for (var i = 0; i < allTapis.length; i++) {
            var p = parseInt(allTapis[i].getAttribute('data-tapis-page'));
            if (p > maxPage) maxPage = p;
        }
        state.maxTapisGroups = maxPage;

        // --- LOGIQUE D'OPTIMISATION AUTO ---
        if (config.autoAjustementTexte) {
            Logger.log("--- DÉBUT DE L'OPTIMISATION AUTO (Disposition & Taille) ---");
            allTapis.forEach(function (t) { t.style.display = 'block'; });

            // 1. Test LIGNE
            Logger.log(">> Test de la disposition : LIGNE");
            setLayoutMode('ligne');
            var sizeNomLigne = measureOptimalSize('.dyn-txt-nom', config.minFontSizeNom);
            var sizeClubLigne = measureOptimalSize('.dyn-txt-club', config.minFontSizeClub);

            // 2. Test COLONNE
            Logger.log(">> Test de la disposition : COLONNE");
            setLayoutMode('colonne');
            var sizeNomColonne = measureOptimalSize('.dyn-txt-nom', config.minFontSizeNom);
            var sizeClubColonne = measureOptimalSize('.dyn-txt-club', config.minFontSizeClub);

            // 3. Choix final
            var finalMode = (sizeNomLigne >= sizeNomColonne) ? 'ligne' : 'colonne';
            Logger.log(">> Disposition finale choisie : " + finalMode.toUpperCase() + " (Critère: taille du nom max)");
            setLayoutMode(finalMode);

            // 4. Appliquer les tailles gagnantes aux Judokas
            var finalSizeNom = (finalMode === 'ligne') ? sizeNomLigne : sizeNomColonne;
            var finalSizeClub = (finalMode === 'ligne') ? sizeClubLigne : sizeClubColonne;

            document.querySelectorAll('.dyn-txt-nom').forEach(function (el) { el.style.fontSize = finalSizeNom + 'px'; });
            document.querySelectorAll('.dyn-txt-club').forEach(function (el) { el.style.fontSize = finalSizeClub + 'px'; });

            // 5. Ajustement indépendant de la Catégorie (Ne rentre pas dans le choix Ligne/Colonne)
            Logger.log(">> Ajustement de la boîte de Catégorie");
            var finalSizeCatTitre = measureOptimalSize('.dyn-txt-cat-titre', config.minFontSizeCatTitre);
            var finalSizeCatSub = measureOptimalSize('.dyn-txt-cat-sub', config.minFontSizeCatSub);

            document.querySelectorAll('.dyn-txt-cat-titre').forEach(function (el) { el.style.fontSize = finalSizeCatTitre + 'px'; });
            document.querySelectorAll('.dyn-txt-cat-sub').forEach(function (el) { el.style.fontSize = finalSizeCatSub + 'px'; });

            Logger.log("--- FIN DE L'OPTIMISATION AUTO ---");
        } else {
            Logger.log("--- OPTIMISATION AUTO DÉSACTIVÉE (Mode forcé par paramètre) ---");
        }

        updateView();
        startTimer();
    }

    // --- Fonctions d'Harmonisation et de Disposition ---

    // Permet de basculer instantanément tout l'affichage entre Ligne et Colonne
    function setLayoutMode(mode) {
        var containers = document.querySelectorAll('.w3-container');

        containers.forEach(function (el) {
            var nom = el.querySelector('.dyn-txt-nom');
            var club = el.querySelector('.dyn-txt-club');

            // --- Judoka 1 (Droite) ---
            if (el.classList.contains('jc-normal-ligne-right') || el.classList.contains('jc-normal-colonne-right')) {
                if (mode === 'ligne') {
                    el.classList.remove('jc-normal-colonne-right'); el.classList.add('jc-normal-ligne-right');
                    if (nom) { nom.classList.remove('order-1'); nom.classList.add('order-2'); }
                    if (club) { club.classList.remove('order-2', 'club-colonne'); club.classList.add('order-1', 'club-ligne-right'); }
                } else {
                    el.classList.remove('jc-normal-ligne-right'); el.classList.add('jc-normal-colonne-right');
                    if (nom) { nom.classList.remove('order-2'); nom.classList.add('order-1'); }
                    if (club) { club.classList.remove('order-1', 'club-ligne-right'); club.classList.add('order-2', 'club-colonne'); }
                }
            }
            // --- Judoka 2 (Gauche) ---
            else if (el.classList.contains('jc-normal-ligne-left') || el.classList.contains('jc-normal-colonne-left')) {
                if (mode === 'ligne') {
                    el.classList.remove('jc-normal-colonne-left'); el.classList.add('jc-normal-ligne-left');
                    if (nom) { nom.classList.remove('order-2'); nom.classList.add('order-1'); }
                    if (club) { club.classList.remove('order-2', 'club-colonne'); club.classList.add('order-2', 'club-ligne-left'); }
                } else {
                    el.classList.remove('jc-normal-ligne-left'); el.classList.add('jc-normal-colonne-left');
                    if (nom) { nom.classList.remove('order-2'); nom.classList.add('order-1'); }
                    if (club) { club.classList.remove('order-2', 'club-ligne-left'); club.classList.add('order-2', 'club-colonne'); }
                }
            }
            // --- En Attente ---
            else if (el.classList.contains('jc-attente-ligne') || el.classList.contains('jc-attente-colonne')) {
                if (mode === 'ligne') {
                    el.classList.remove('jc-attente-colonne'); el.classList.add('jc-attente-ligne');
                } else {
                    el.classList.remove('jc-attente-ligne'); el.classList.add('jc-attente-colonne');
                }
            }
        });
    }

    // Calcule la plus grande taille possible où aucun texte ne déborde
    function measureOptimalSize(selector, minSizePx) {
        var elements = document.querySelectorAll(selector);
        if (!elements.length) {
            Logger.warn("Mesure (" + selector + ") : Aucun élément trouvé.");
            return minSizePx;
        }

        // Reset pour s'assurer qu'aucun style inline résiduel ne fausse la mesure initiale
        for (var i = 0; i < elements.length; i++) {
            elements[i].style.fontSize = '';
        }

        // On détermine la taille de départ (Max théorique du CSS) sur le 1er élément
        var computedStyle = window.getComputedStyle(elements[0]);
        var baseSizePx = parseFloat(computedStyle.fontSize);
        if (isNaN(baseSizePx)) baseSizePx = 30; // Fallback de sécurité

        var tailleMinOptimale = baseSizePx;
        var hasElements = false;

        Logger.log(">> Début mesure pour " + selector + " | Max théorique (CSS): " + baseSizePx + "px | Min autorisé: " + minSizePx + "px");

        // Utilisation d'une boucle for classique pour pouvoir utiliser 'break'
        for (var i = 0; i < elements.length; i++) {
            var el = elements[i];

            // OPTIMISATION ULTIME : Si on a déjà touché le fond lors d'un test précédent, on arrête tout !
            if (tailleMinOptimale <= minSizePx) {
                Logger.log("   [STOP] Taille minimum absolue (" + minSizePx + "px) atteinte. Inutile de tester le reste.");
                tailleMinOptimale = minSizePx;
                break;
            }

            // On teste avec le record actuel
            el.style.fontSize = tailleMinOptimale + "px";
            var espaceDispo = el.clientWidth || (el.parentElement ? el.parentElement.clientWidth : 0);

            // Si ça rentre, on passe directement au judoka suivant
            if (el.scrollWidth <= espaceDispo) {
                hasElements = true;
                continue;
            }

            // S'il y a dépassement, on trace
            var texteExtrait = el.innerText.replace(/\n/g, ' ').trim().substring(0, 15);
            Logger.log("   [DÉPASSEMENT] Élément '" + texteExtrait + "...'");
            Logger.log("     -> Ne rentre pas à " + tailleMinOptimale + "px (scrollWidth: " + el.scrollWidth + "px > espace: " + espaceDispo + "px)");

            var fontSizeTemp = tailleMinOptimale;

            // Boucle de réduction pixel par pixel
            while (el.scrollWidth > espaceDispo && fontSizeTemp > minSizePx) {
                fontSizeTemp -= 1;
                el.style.fontSize = fontSizeTemp + "px";
            }

            // Mise à jour du nouveau plafond
            if (fontSizeTemp < tailleMinOptimale) {
                tailleMinOptimale = fontSizeTemp;
                Logger.log("     -> [AJUSTEMENT] Nouvelle taille retenue : " + tailleMinOptimale + "px");
            }
            hasElements = true;
        }

        // Nettoyage post-mesure pour laisser un DOM propre pour le prochain test
        for (var i = 0; i < elements.length; i++) {
            elements[i].style.fontSize = '';
        }

        if (!hasElements) return minSizePx;
        Logger.log(">> Fin mesure pour " + selector + " | Taille finale retenue : " + tailleMinOptimale + "px");
        return tailleMinOptimale;
    }
    

    // --- Gestion du Timer et Barre de progression ---
    function startTimer() {
        var timeLeft = 0;
        var intervalStep = 100; // ms
        var totalSteps = (config.dureeRotation * 1000) / intervalStep;

        if (state.timer) clearInterval(state.timer);

        state.timer = setInterval(function () {
            timeLeft++;

            // Mise à jour de la barre visuelle
            if (state.progressBar) {
                var percent = (timeLeft / totalSteps) * 100;
                state.progressBar.style.width = percent + "%";
            }

            // Fin du décompte
            if (timeLeft >= totalSteps) {
                // On retarde le changement de 100ms pour laisser la transition CSS toucher le bord
                setTimeout(function () {
                    var isCycleFinished = nextStep();

                    // Remise à zéro instantanée de la barre (sans animation de recul)
                    // On ne remet la barre à zéro QUE si on continue à tourner
                    if (!isCycleFinished && state.progressBar) {
                        state.progressBar.style.transition = 'none';
                        state.progressBar.style.width = '0%';
                        void state.progressBar.offsetWidth; // Force l'application immédiate du 0%
                        state.progressBar.style.transition = 'width 0.1s linear';
                    }
                }, intervalStep);

                timeLeft = 0;
            }
        }, intervalStep);
    }

    // --- Logique de passage à l'étape suivante ---
    function nextStep() {
        // 1. On avance d'une page de combats
        state.currentCombatPage++;

        // 2. Si on dépasse le max de pages pour la vue actuelle (le tapis le plus chargé)
        if (state.currentCombatPage > state.maxCombatPagesCurrentView) {

            // On a fini le tour des combats pour ce groupe de tapis
            state.currentCombatPage = 1;
            state.currentTapisGroupIndex++;

            // 3. Si on a fait tous les groupes de tapis
            if (state.currentTapisGroupIndex > state.maxTapisGroups) {

                // 1. Coupe le moteur
                if (state.timer) clearInterval(state.timer);

                // Redirection vers la page source pour réévaluer la configuration
                if (config.urlRedirecteur && config.urlRedirecteur.trim() !== '') {
                    window.location.href = config.urlRedirecteur;
                } else {
                    window.location.reload(true); // Fallback
                }
                return true; // Cycle terminé
            }
        }

        // Mise à jour de l'affichage
        updateView();
        return false; // Cycle en cours
    }

    // --- Mise à jour de l'affichage (DOM) --

    function updateView() {
        // 1. Gestion des Tapis (Masquer/Afficher les blocs Tapis entiers)
        var allTapis = document.querySelectorAll('.tapis-card');
        var visibleTapis = [];

        for (var i = 0; i < allTapis.length; i++) {
            var div = allTapis[i];
            var page = parseInt(div.getAttribute('data-tapis-page'));

            if (page === state.currentTapisGroupIndex) {
                if (div.style.display !== 'block') {
                    div.style.display = 'block'; // Un simple block suffit dans un parent Flexbox
                    div.classList.remove('w3-animate-opacity');
                    void div.offsetWidth;
                    div.classList.add('w3-animate-opacity');
                }
                visibleTapis.push(div);
            } else {
                div.style.display = 'none'; // CRUCIAL : Masquer les autres tapis pour libérer la place
            }
        }

        // 2. Calcul du nombre max de pages de combats pour ce groupe visible
        var maxPagesLocal = 1;
        for (var i = 0; i < visibleTapis.length; i++) {
            var tapisDiv = visibleTapis[i];
            var rows = tapisDiv.querySelectorAll('.grid-combat-row');
            var nbPagesCeTapis = Math.ceil(rows.length / config.combatsParPage) || 1;
            if (nbPagesCeTapis > maxPagesLocal) {
                maxPagesLocal = nbPagesCeTapis;
            }
        }
        state.maxCombatPagesCurrentView = maxPagesLocal;

        // 3. Affichage des lignes de combats pour chaque tapis visible
        for (var i = 0; i < visibleTapis.length; i++) {
            var tapisDiv = visibleTapis[i];
            var rows = tapisDiv.querySelectorAll('.grid-combat-row');
            var localMaxPage = Math.ceil(rows.length / config.combatsParPage) || 1;

            // Fige le tapis sur sa dernière page en attendant que le tapis voisin termine
            var targetLocalPage = Math.min(state.currentCombatPage, localMaxPage);

            // Calcul des index de lignes
            var minIndex = (targetLocalPage - 1) * config.combatsParPage + 1;
            var maxIndex = targetLocalPage * config.combatsParPage;

            // Mise à jour de l'indicateur de pages (Puces visuelles)
            var indicator = tapisDiv.querySelector("[id^='paging_indicator']");
            if (indicator) {
                if (localMaxPage > 1) {
                    indicator.innerHTML = generatePaginationDots(targetLocalPage, localMaxPage);
                    indicator.style.display = 'flex'; // On force Flexbox pour un alignement parfait
                } else {
                    indicator.style.display = 'none'; // Cache si 1/1
                }
            }

            // Masquer / Afficher les lignes <tr>
            for (var r = 0; r < rows.length; r++) {
                var row = rows[r];
                var rowIdx = parseInt(row.getAttribute('data-row-index'));
                row.style.display = (rowIdx >= minIndex && rowIdx <= maxIndex) ? '' : 'none';
            }
        }
    }

    // Démarrage au chargement du DOM
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }

})();