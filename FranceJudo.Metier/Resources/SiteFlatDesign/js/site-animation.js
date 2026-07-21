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
        minFontSizeCatSub: 10,    // Limite basse pour le niveau/tour
        boostRatioTexte: 1.25    // Multiplicateur pour tenter d'agrandir la police CSS par défaut (1.25 = +25%)
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

        // --- LOGIQUE D'OPTIMISATION AUTO (Proportionnelle) ---
        if (config.autoAjustementTexte) {
            Logger.log("--- DÉBUT DE L'OPTIMISATION AUTO ---");
            allTapis.forEach(function (t) { t.style.display = 'block'; });

            var containersGrid = document.querySelectorAll('.combat-grid-container');
            containersGrid.forEach(function (c) { c.style.height = 'auto'; }); // Empêche l'écrasement vertical

            // 1. Simulation LIGNE
            var resultLigne = testLayoutMode('ligne');
            Logger.log(">> LIGNE   -> Score Idéal: " + resultLigne.idealNomSize.toFixed(1) + "px | Taille Appliquée: " + resultLigne.nomSize.toFixed(1) + "px");

            // 2. Simulation COLONNE
            var resultColonne = testLayoutMode('colonne');
            Logger.log(">> COLONNE -> Score Idéal: " + resultColonne.idealNomSize.toFixed(1) + "px | Taille Appliquée: " + resultColonne.nomSize.toFixed(1) + "px");

            // 3. Choix : On compare la taille IDÉALE (mathématique) pour savoir qui a vraiment le plus de place
            var bestMode = 'colonne'; // Fallback par défaut

            // CRITÈRE 1 : La plus grande taille absolue l'emporte
            if (resultLigne.idealNomSize !== resultColonne.idealNomSize) {
                bestMode = (resultLigne.idealNomSize > resultColonne.idealNomSize) ? 'ligne' : 'colonne';
                Logger.log(">> Victoire par Taille Absolue : " + bestMode.toUpperCase());
            }
            // CRITÈRE 2 : Égalité
            else {
                // Sous-critère A : Égalité AU PLANCHER (minNom)
                if (resultLigne.idealNomSize === config.minFontSizeNom) {

                    if (!resultLigne.isOverflowing && resultColonne.isOverflowing) {
                        bestMode = 'ligne'; // Ligne tient parfaitement, Colonne est coupée
                        Logger.log(">> Égalité au plancher - Victoire par non-débordement : LIGNE");
                    }
                    else if (resultLigne.isOverflowing && !resultColonne.isOverflowing) {
                        bestMode = 'colonne'; // Colonne tient parfaitement, Ligne est coupée
                        Logger.log(">> Égalité au plancher - Victoire par non-débordement : COLONNE");
                    }
                    else {
                        // Les deux sont coupés, ou aucun n'est coupé -> Règle du Ratio
                        var ratioL = resultLigne.idealNomSize / resultLigne.baseNom;
                        var ratioC = resultColonne.idealNomSize / resultColonne.baseNom;
                        bestMode = (ratioL >= ratioC) ? 'ligne' : 'colonne';
                        Logger.log(">> Égalité au plancher & Débordement équivalent - Victoire par Ratio : " + bestMode.toUpperCase());
                    }
                }
                // Sous-critère B : Égalité HORS PLANCHER
                else {
                    var ratioL = resultLigne.idealNomSize / resultLigne.baseNom;
                    var ratioC = resultColonne.idealNomSize / resultColonne.baseNom;
                    bestMode = (ratioL >= ratioC) ? 'ligne' : 'colonne';
                    Logger.log(">> Égalité de taille pure - Victoire par Ratio : " + bestMode.toUpperCase());
                }
            }

            Logger.log(">> Disposition finale choisie : " + bestMode.toUpperCase());
            setLayoutMode(bestMode);
            var bestResult = (bestMode === 'ligne') ? resultLigne : resultColonne;

            // 4. Application
            document.querySelectorAll('.dyn-txt-nom').forEach(function (el) { el.style.fontSize = bestResult.nomSize + 'px'; });
            document.querySelectorAll('.dyn-txt-club').forEach(function (el) { el.style.fontSize = bestResult.clubSize + 'px'; });

            // 5. Ajustement Catégorie
            adjustCategorySizes();

            containersGrid.forEach(function (c) { c.style.height = ''; }); // Restauration visuelle
            Logger.log("--- FIN DE L'OPTIMISATION AUTO ---");
        } else {
            Logger.log("--- OPTIMISATION AUTO DÉSACTIVÉE ---");
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

    // Teste une disposition avec l'algorithme HYBRIDE (+ Boost de taille initiale)
    function testLayoutMode(mode) {
        setLayoutMode(mode);
        document.body.offsetHeight; // Force le rendu visuel

        var containers = document.querySelectorAll('.judoka-box .w3-container');
        var nomEls = document.querySelectorAll('.dyn-txt-nom');
        var clubEls = document.querySelectorAll('.dyn-txt-club');

        var baseNom = nomEls.length ? parseFloat(window.getComputedStyle(nomEls[0]).fontSize) || 30 : 30;
        var baseClub = clubEls.length ? parseFloat(window.getComputedStyle(clubEls[0]).fontSize) || 15 : 15;
        var ratioClub = baseClub / baseNom;

        var minNom = config.minFontSizeNom;
        var minClub = config.minFontSizeClub;

        // Appel depuis la configuration
        var currentNom = Math.floor(baseNom * config.boostRatioTexte);
        var currentClub = Math.floor(baseClub * config.boostRatioTexte);

        Logger.log("==================================================");
        Logger.log(">> DÉMARRAGE HYBRIDE [" + mode.toUpperCase() + "] | Départ Nom=" + currentNom + "px (Boost x" + config.boostRatioTexte + ")");
        Logger.log("==================================================");

        var oldStyles = [];
        for (var i = 0; i < containers.length; i++) {
            var p = containers[i];
            if (p.clientWidth === 0) continue;

            p.setAttribute('data-dispo', p.clientWidth - 4);

            var n = p.querySelector('.dyn-txt-nom');
            var c = p.querySelector('.dyn-txt-club');

            oldStyles.push({
                p: p, n: n, c: c,
                oO: p.style.overflow, oD: p.style.display, oW: p.style.width, oA: p.style.alignSelf,
                nW: n ? n.style.width : '', nS: n ? n.style.flexShrink : '', nT: n ? n.style.textOverflow : '', nA: n ? n.style.alignSelf : '',
                cW: c ? c.style.width : '', cS: c ? c.style.flexShrink : '', cT: c ? c.style.textOverflow : '', cA: c ? c.style.alignSelf : ''
            });

            p.style.display = 'inline-flex';
            p.style.width = 'max-content';
            p.style.alignSelf = 'flex-start';
            p.style.overflow = 'visible';

            if (n) { n.style.width = 'max-content'; n.style.flexShrink = '0'; n.style.textOverflow = 'clip'; n.style.alignSelf = 'flex-start'; }
            if (c) { c.style.width = 'max-content'; c.style.flexShrink = '0'; c.style.textOverflow = 'clip'; c.style.alignSelf = 'flex-start'; }
        }

        for (var i = 0; i < containers.length; i++) {
            if (currentNom <= minNom) {
                Logger.log("   [STOP] Plancher (" + minNom + "px) atteint.");
                currentNom = minNom;
                break;
            }

            var p = containers[i];
            if (!p.hasAttribute('data-dispo')) continue;

            var n = p.querySelector('.dyn-txt-nom');
            var c = p.querySelector('.dyn-txt-club');

            if (n) n.style.fontSize = currentNom + 'px';
            if (c) c.style.fontSize = currentClub + 'px';

            var espaceDispo = parseFloat(p.getAttribute('data-dispo'));
            var required = p.scrollWidth;

            var txt = n ? n.innerText.replace(/\n/g, ' ').trim().substring(0, 15) : 'N/A';

            if (required <= espaceDispo) {
                continue; // Le Boost passe sans problème pour ce judoka !
            }

            Logger.log("   [DÉBORDEMENT] '" + txt + "...' -> Requis: " + required + "px > Dispo: " + espaceDispo + "px");

            var scale = espaceDispo / required;
            currentNom = Math.floor(currentNom * scale);
            currentClub = Math.max(Math.floor(currentNom * ratioClub), minClub);

            if (n) n.style.fontSize = currentNom + 'px';
            if (c) c.style.fontSize = currentClub + 'px';

            required = p.scrollWidth;

            var loopCount = 0;
            Logger.log("      -> [ÉTAPE B] Post-ratio: Requis " + required + "px | Dispo " + espaceDispo + "px");

            while (required > espaceDispo && currentNom > minNom) {
                loopCount++;
                currentNom--;
                currentClub = Math.max(Math.floor(currentNom * ratioClub), minClub);

                if (n) n.style.fontSize = currentNom + 'px';
                if (c) c.style.fontSize = currentClub + 'px';

                required = p.scrollWidth;
                Logger.log("         - Baisse à " + currentNom + "px (Nouvel encombrement: " + required + "px)");
            }

            Logger.log("      -> [RÉSOLU] Plafond verrouillé à : " + currentNom + "px");
        }

        // --- NOUVEAU : Détection du débordement final ---
        var isOverflowing = false;
        for (var i = 0; i < containers.length; i++) {
            var p = containers[i];
            if (p.hasAttribute('data-dispo') && p.scrollWidth > parseFloat(p.getAttribute('data-dispo'))) {
                isOverflowing = true;
                break; // Dès qu'un seul judoka déborde, le layout est considéré comme overflow
            }
        }

        for (var i = 0; i < oldStyles.length; i++) {
            var s = oldStyles[i];
            s.p.style.display = s.oD;
            s.p.style.width = s.oW;
            s.p.style.alignSelf = s.oA;
            s.p.style.overflow = s.oO;
            if (s.n) { s.n.style.width = s.nW; s.n.style.flexShrink = s.nS; s.n.style.textOverflow = s.nT; s.n.style.alignSelf = s.nA; s.n.style.fontSize = ''; }
            if (s.c) { s.c.style.width = s.cW; s.c.style.flexShrink = s.cS; s.c.style.textOverflow = s.cT; s.c.style.alignSelf = s.cA; s.c.style.fontSize = ''; }
        }

        return {
            mode: mode,
            nomSize: Math.max(currentNom, minNom),
            clubSize: Math.max(currentClub, minClub),
            idealNomSize: Math.max(currentNom, minNom), // On assure que idealNomSize plafonne bien au minNom
            baseNom: baseNom,
            isOverflowing: isOverflowing // On remonte l'information cruciale
        };
    }

    // Ajustement des catégories (Calcul INDÉPENDANT avec lecture directe de l'Espace Interne)
    // Ajustement des catégories (Calcul INDÉPENDANT avec lecture directe de l'Espace Interne)
    function adjustCategorySizes() {
        var boxes = document.querySelectorAll('.cat-box');
        var titres = document.querySelectorAll('.dyn-txt-cat-titre');
        var subs = document.querySelectorAll('.dyn-txt-cat-sub');
        var eqs = document.querySelectorAll('.cartouche-equipe'); // On cible le cartouche complet !

        if (!titres.length) return;

        // VÉRIFICATION : Y a-t-il des combats par équipe à l'écran ?
        var hasEq = eqs.length > 0;

        var baseTitre = parseFloat(window.getComputedStyle(titres[0]).fontSize) || 20;
        var baseSub = subs.length ? parseFloat(window.getComputedStyle(subs[0]).fontSize) || 15 : 15;
        var baseEq = hasEq ? parseFloat(window.getComputedStyle(eqs[0]).fontSize) || 12 : 12;

        var minTitre = config.minFontSizeCatTitre || 12;
        var minSub = config.minFontSizeCatSub || 10;
        var boostRatio = config.boostRatioTexte || 1.25;

        var currentTitre = Math.floor(baseTitre * boostRatio);
        var currentSub = Math.floor(baseSub * boostRatio);
        var currentEq = hasEq ? Math.floor(baseEq * boostRatio) : 0;

        Logger.log("==================================================");
        Logger.log(">> DÉMARRAGE HYBRIDE [CATÉGORIE] (Moteur Padding Interne)");
        Logger.log(">> Départ Titre=" + currentTitre + "px, Sub=" + currentSub + "px" + (hasEq ? ", Equipe=" + currentEq + "px" : ""));
        Logger.log("==================================================");

        // 1. PRÉPARATION
        var oldStyles = [];
        for (var i = 0; i < boxes.length; i++) {
            var box = boxes[i];
            if (box.clientWidth === 0) continue;

            var compStyle = window.getComputedStyle(box);
            var paddingX = parseFloat(compStyle.paddingLeft) + parseFloat(compStyle.paddingRight);
            var espaceInterne = box.clientWidth - paddingX - 2;

            box.setAttribute('data-dispo', espaceInterne);

            var divs = box.querySelectorAll('div');
            var divStyles = [];
            for (var d = 0; d < divs.length; d++) {
                divStyles.push({
                    d: divs[d].style.display,
                    w: divs[d].style.width,
                    f: divs[d].style.flexShrink,
                    t: divs[d].style.textOverflow
                });

                divs[d].style.display = 'inline-block';
                divs[d].style.width = 'max-content';
                divs[d].style.flexShrink = '0';
                divs[d].style.textOverflow = 'clip';
            }
            oldStyles.push({ box: box, divs: divs, divStyles: divStyles });
        }

        // 2. BOUCLE DE MESURE
        for (var i = 0; i < boxes.length; i++) {
            // Condition de sortie anticipée robuste (ignore l'équipe si absente)
            var limitTitre = currentTitre <= minTitre;
            var limitSub = currentSub <= minSub;
            var limitEq = hasEq ? (currentEq <= minSub) : true;

            if (limitTitre && limitSub && limitEq) {
                break;
            }

            var box = boxes[i];
            if (!box.hasAttribute('data-dispo')) continue;

            var tEl = box.querySelector('.dyn-txt-cat-titre');
            var sEl = box.querySelector('.dyn-txt-cat-sub');
            var eEl = box.querySelector('.cartouche-equipe'); // Le JS manipule le parent !
            var espaceDispo = parseFloat(box.getAttribute('data-dispo'));

            // --- A. Ajustement du TITRE ---
            if (tEl && currentTitre > minTitre) {
                tEl.style.fontSize = currentTitre + 'px';
                var reqT = tEl.scrollWidth;
                if (reqT > espaceDispo) {
                    var scaleT = espaceDispo / reqT;
                    currentTitre = Math.max(Math.floor(currentTitre * scaleT), minTitre);
                    tEl.style.fontSize = currentTitre + 'px';
                    reqT = tEl.scrollWidth;
                    while (reqT > espaceDispo && currentTitre > minTitre) {
                        currentTitre--;
                        tEl.style.fontSize = currentTitre + 'px';
                        reqT = tEl.scrollWidth;
                    }
                }
            }

            // --- B. Ajustement du SOUS-TITRE ---
            if (sEl && currentSub > minSub) {
                sEl.style.fontSize = currentSub + 'px';
                var reqS = sEl.scrollWidth;
                if (reqS > espaceDispo) {
                    var scaleS = espaceDispo / reqS;
                    currentSub = Math.max(Math.floor(currentSub * scaleS), minSub);
                    sEl.style.fontSize = currentSub + 'px';
                    reqS = sEl.scrollWidth;
                    while (reqS > espaceDispo && currentSub > minSub) {
                        currentSub--;
                        sEl.style.fontSize = currentSub + 'px';
                        reqS = sEl.scrollWidth;
                    }
                }
            }

            // --- C. Ajustement du CARTOUCHE EQUIPE ---
            if (hasEq && eEl && currentEq > minSub) {
                eEl.style.fontSize = currentEq + 'px';
                var reqE = eEl.scrollWidth;
                if (reqE > espaceDispo) {
                    var scaleE = espaceDispo / reqE;
                    currentEq = Math.max(Math.floor(currentEq * scaleE), minSub);
                    eEl.style.fontSize = currentEq + 'px';
                    reqE = eEl.scrollWidth;
                    while (reqE > espaceDispo && currentEq > minSub) {
                        currentEq--;
                        eEl.style.fontSize = currentEq + 'px';
                        reqE = eEl.scrollWidth;
                    }
                }
            }
        }

        // 3. RESTAURATION
        for (var i = 0; i < oldStyles.length; i++) {
            var s = oldStyles[i];
            for (var d = 0; d < s.divs.length; d++) {
                s.divs[d].style.display = s.divStyles[d].d;
                s.divs[d].style.width = s.divStyles[d].w;
                s.divs[d].style.flexShrink = s.divStyles[d].f;
                s.divs[d].style.textOverflow = s.divStyles[d].t;
                s.divs[d].style.fontSize = '';
            }
        }

        // Application finale
        titres.forEach(function (t) { t.style.fontSize = currentTitre + 'px'; });
        subs.forEach(function (s) { s.style.fontSize = currentSub + 'px'; });
        if (hasEq) {
            eqs.forEach(function (e) { e.style.fontSize = currentEq + 'px'; });
        }

        Logger.log(">> BILAN FINAL [CATÉGORIE] -> Titre : " + currentTitre + "px | Sub : " + currentSub + "px" + (hasEq ? " | Equipe : " + currentEq + "px" : ""));
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
                    Logger.log(">> Going back to  : '" + config.urlRedirecteur + "'");
                    window.location.href = config.urlRedirecteur;
                } else {
                    Logger.log(">> No redirect, reloading current");
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