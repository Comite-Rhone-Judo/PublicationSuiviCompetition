(function() {
    // --- Configuration et Etat ---
    var config = {
        dureeRotation: 10,      // Valeur par défaut
        combatsParPage: 5,      // Valeur par défaut
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

    // --- Template Dédié : Générateur de puces de pagination ---
    function generatePaginationDots(currentPage, totalPages) {
        var html = '';
        for (var i = 1; i <= totalPages; i++) {
            if (i === currentPage) {
                // CORRECTION : Puce active avec notre classe d'animation "w3-dot-active"
                html += '<span class="w3-text-orange w3-dot-active" style="font-size: 1.2em;">&#9679;</span>';
            } else {
                // Puce inactive (Blanche, semi-transparente, statique)
                html += '<span class="w3-text-white" style="font-size: 1.2em; opacity: 0.3; transition: opacity 0.3s;">&#9679;</span>';
            }
        }
        return html;
    }

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
                // On retarde le changement de 100ms pour laisser la transition CSS toucher le bord
                setTimeout(function () {
                    nextStep();

                    // Remise à zéro instantanée de la barre (sans animation de recul)
                    if (state.progressBar) {
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
            var rows = tapisDiv.querySelectorAll('.combat-row');
            var nbPagesCeTapis = Math.ceil(rows.length / config.combatsParPage) || 1;
            if (nbPagesCeTapis > maxPagesLocal) {
                maxPagesLocal = nbPagesCeTapis;
            }
        }
        state.maxCombatPagesCurrentView = maxPagesLocal;

        // 3. Affichage des lignes de combats pour chaque tapis visible
        for (var i = 0; i < visibleTapis.length; i++) {
            var tapisDiv = visibleTapis[i];
            var rows = tapisDiv.querySelectorAll('.combat-row');
            var localMaxPage = Math.ceil(rows.length / config.combatsParPage) || 1;

            // CORRECTION : Au lieu de repartir à 1 (modulo), on fige le tapis sur sa dernière page 
            // (ex: Page 2 sur 2) en attendant que le tapis voisin termine son propre cycle.
            var targetLocalPage = Math.min(state.currentCombatPage, localMaxPage);

            // Calcul des index de lignes
            var minIndex = (targetLocalPage - 1) * config.combatsParPage + 1;

            var maxIndex = targetLocalPage * config.combatsParPage;


            // Mise à jour de l'indicateur "Page X sur Y"
            /*
            var indicator = tapisDiv.querySelector("[id^='paging_indicator']");
            if (indicator) {
                if (localMaxPage > 1) {
                    // CORRECTION : Utilisation de innerHTML avec un <br/> pour forcer les 2 blocs symétriques
                    indicator.innerHTML = "Page<br/>" + targetLocalPage + " sur " + localMaxPage;
                    indicator.style.display = '';
                } else {
                    indicator.style.display = 'none'; // Cache si 1/1
                }
            }
            */
            // Mise à jour de l'indicateur de pages (Puces visuelles)
            var indicator = tapisDiv.querySelector("[id^='paging_indicator']");
            if (indicator) {
                if (localMaxPage > 1) {
                    // CORRECTION : On appelle notre générateur de puces
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
                // Utilisation de la condition ternaire pour l'affichage
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