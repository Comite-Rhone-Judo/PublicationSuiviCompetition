(function() {
    // --- Configuration et Etat ---
    var config = {
        dureeRotation: 10,      // Valeur par défaut
        combatsParPage: 8,      // Valeur par défaut
        layoutMode: 4           // Valeur par défaut
    };

    var state = {
        currentTapisGroupIndex: 1,    // Page de tapis courante (1, 2, ...)
        maxTapisGroups: 1,            // Nombre total de pages de tapis
        currentCombatPage: 1,         // Page de combat interne courante (1, 2, ...)
        maxCombatPagesCurrentView: 1, // Max pages de combats parmi les tapis affichés
        timer: null,
        progressBar: null
    };

    // --- Initialisation ---
    function init() {
        var container = document.getElementById('main-container');
        state.progressBar = document.getElementById('progress-bar');

        if (container) {
            // Lecture des paramètres depuis les attributs data- du XSLT
            config.dureeRotation = parseInt(container.getAttribute('data-duree-rotation')) || 10;
            config.combatsParPage = parseInt(container.getAttribute('data-combats-par-page')) || 8;
            config.layoutMode = parseInt(container.getAttribute('data-layout-mode')) || 4;
        }

        // Calculer le nombre total de groupes de tapis
        var allTapis = document.querySelectorAll('.tapis-card');
        var maxPage = 0;
        for (var i = 0; i < allTapis.length; i++) {
            var p = parseInt(allTapis[i].getAttribute('data-tapis-page'));
            if (p > maxPage) maxPage = p;
        }
        state.maxTapisGroups = maxPage;

        console.log("Animation Init: Groupes Tapis=" + state.maxTapisGroups + 
                    ", Durée=" + config.dureeRotation + "s" + 
                    ", Combats/Page=" + config.combatsParPage);

        // Lancer le premier affichage
        updateView();

        // Démarrer le timer
        startTimer();
    }

    // --- Gestion du Timer et Barre de progression ---
    function startTimer() {
        var timeLeft = 0;
        var intervalStep = 100; // ms
        var totalSteps = (config.dureeRotation * 1000) / intervalStep;

        if (state.timer) clearInterval(state.timer);

        state.timer = setInterval(function() {
            timeLeft++;
            
            // Mise à jour de la barre visuelle
            if (state.progressBar) {
                var percent = (timeLeft / totalSteps) * 100;
                state.progressBar.style.width = percent + "%";
            }

            // Fin du décompte
            if (timeLeft >= totalSteps) {
                nextStep();
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
                console.log("Cycle terminé. Rechargement de la page...");
                // Force le rechargement depuis le serveur pour avoir les nouvelles données XML
                window.location.reload(true); 
                return;
            }
        }

        // Mise à jour de l'affichage
        updateView();
    }

    // --- Mise à jour de l'affichage (DOM) ---
    function updateView() {
        // 1. Gestion des Tapis (Masquer/Afficher les blocs Tapis entiers)
        var allTapis = document.querySelectorAll('.tapis-card');
        var visibleTapis = [];

        for (var i = 0; i < allTapis.length; i++) {
            var div = allTapis[i];
            var page = parseInt(div.getAttribute('data-tapis-page'));

            if (page === state.currentTapisGroupIndex) {
                div.style.display = 'block';
                // Petit effet de fade-in si w3.css animate est présent
                div.classList.remove('w3-animate-opacity'); 
                void div.offsetWidth; // Trigger reflow
                div.classList.add('w3-animate-opacity');
                
                visibleTapis.push(div);
            } else {
                div.style.display = 'none';
            }
        }

        // 2. Calcul du nombre max de pages de combats pour ce groupe visible
        // (Ex: Tapis 1 a 2 pages, Tapis 2 a 5 pages -> On doit attendre 5 cycles)
        var maxPagesLocal = 1;

        for (var i = 0; i < visibleTapis.length; i++) {
            var tapisDiv = visibleTapis[i];
            var rows = tapisDiv.querySelectorAll('.combat-row');
            var totalCombats = rows.length;
            
            var nbPagesCeTapis = Math.ceil(totalCombats / config.combatsParPage);
            if (nbPagesCeTapis === 0) nbPagesCeTapis = 1; // Toujours au moins 1 page (vide ou non)

            if (nbPagesCeTapis > maxPagesLocal) {
                maxPagesLocal = nbPagesCeTapis;
            }
        }
        state.maxCombatPagesCurrentView = maxPagesLocal;

        // 3. Affichage des lignes de combats pour chaque tapis visible
        for (var i = 0; i < visibleTapis.length; i++) {
            var tapisDiv = visibleTapis[i];
            var rows = tapisDiv.querySelectorAll('.combat-row');
            var totalCombats = rows.length;
            var localMaxPage = Math.ceil(totalCombats / config.combatsParPage) || 1;

            // Logique de cycle infini pour les tapis ayant moins de combats :
            // Si on est à la page globale 3 mais que le tapis n'a que 2 pages, on affiche la page 1.
            // Formule : (PageGlobale - 1) MODULO MaxPagesLocal + 1
            var pageIndex0 = state.currentCombatPage - 1; 
            var targetLocalPage0 = pageIndex0 % localMaxPage; 
            var targetLocalPage = targetLocalPage0 + 1; 

            // Calcul des index de lignes (1-based)
            var minIndex = (targetLocalPage - 1) * config.combatsParPage + 1;
            var maxIndex = targetLocalPage * config.combatsParPage;

            // Mise à jour de l'indicateur visuel (ex: "Page 1/3")
            // On cherche un élément dont l'ID commence par paging_indicator dans ce tapis
            var indicator = tapisDiv.querySelector("[id^='paging_indicator']");
            if (indicator) {
                indicator.innerText = targetLocalPage + "/" + localMaxPage;
            }

            // Masquer / Afficher les lignes <tr>
            for (var r = 0; r < rows.length; r++) {
                var row = rows[r];
                // On utilise l'attribut calculé par XSLT pour la position
                // Note: on utilise querySelector ou l'attribut direct si disponible
                // Ici on suppose que le XSLT a généré data-row-index="{position()}"
                var rowIdx = parseInt(row.getAttribute('data-row-index'));

                if (rowIdx >= minIndex && rowIdx <= maxIndex) {
                    row.style.display = ''; // Affiche (valeur par défaut du navigateur pour table-row)
                } else {
                    row.style.display = 'none';
                }
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