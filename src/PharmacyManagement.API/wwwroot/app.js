const API_BASE_URL = '/api';

// Vérifier l'authentification
function checkAuth() {
    const token = localStorage.getItem('token');
    if (!token) {
        window.location.href = '/login.html';
        return false;
    }
    return true;
}

// Ajouter le token aux requêtes
function getAuthHeaders() {
    const token = localStorage.getItem('token');
    return {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`
    };
}

// Intercepter les fetch pour ajouter le token et gérer les erreurs d'authentification
const originalFetch = window.fetch;
window.fetch = async function(url, options = {}) {
    if (url.startsWith('/api/') && !url.includes('/auth/login') && !url.includes('/auth/register') && !url.includes('/setup/')) {
        const token = localStorage.getItem('token');
        if (token) {
            options.headers = {
                'Content-Type': 'application/json',
                ...options.headers,
                'Authorization': `Bearer ${token}`
            };
        } else {
            // S'assurer que Content-Type est défini même sans token
            if (!options.headers || !options.headers['Content-Type']) {
                options.headers = {
                    'Content-Type': 'application/json',
                    ...options.headers
                };
            }
        }
    }
    
    try {
        const response = await originalFetch(url, options);
        
        // Si l'authentification échoue, rediriger vers la page de connexion
        if (response.status === 401 || response.status === 403) {
            localStorage.removeItem('token');
            localStorage.removeItem('user');
            localStorage.removeItem('refreshToken');
            if (!window.location.pathname.includes('login.html')) {
                window.location.href = '/login.html';
            }
        }
        
        return response;
    } catch (error) {
        // Gérer les erreurs réseau (Failed to fetch)
        console.error('Erreur réseau lors de la requête:', url, error);
        throw error;
    }
};

// Gestion des onglets
document.querySelectorAll('.tab-btn').forEach(btn => {
    btn.addEventListener('click', () => {
        const tab = btn.dataset.tab;
        switchTab(tab);
    });
});

function switchTab(tabName) {
    // Désactiver tous les onglets
    document.querySelectorAll('.tab-btn').forEach(btn => btn.classList.remove('active'));
    document.querySelectorAll('.tab-content').forEach(content => content.classList.remove('active'));
    
    // Activer l'onglet sélectionné
    document.querySelector(`[data-tab="${tabName}"]`).classList.add('active');
    document.getElementById(tabName).classList.add('active');
    
    // Charger les données
    loadTabData(tabName);
}

function loadTabData(tabName) {
    switch(tabName) {
        case 'dashboard':
            loadDashboard();
            break;
        case 'medicaments':
            loadMedicaments();
            break;
        case 'clients':
            loadClients();
            break;
        case 'ventes':
            loadVentes();
            break;
        case 'stocks':
            loadStocks();
            break;
        case 'commandes':
            loadCommandes();
            break;
        case 'fournisseurs':
            loadFournisseurs();
            break;
        case 'prescriptions':
            loadPrescriptions();
            break;
        case 'alertes':
            loadAlertes();
            break;
    }
}

// Charger le dashboard
async function loadDashboard() {
    const container = document.getElementById('dashboard-content');
    container.innerHTML = '<div class="loading">Chargement du tableau de bord...</div>';
    
    try {
        const response = await fetch(`${API_BASE_URL}/dashboard`);
        const dashboard = await response.json();
        
        // Valeurs par défaut pour éviter les erreurs
        const chiffreAffaires = (dashboard.chiffreAffairesTotal || 0);
        const medicamentsPopulaires = dashboard.medicamentsPopulaires || [];
        const ventesMensuelles = dashboard.ventesMensuelles || [];
        
        container.innerHTML = `
            <div class="stats-grid">
                <div class="stat-card">
                    <div class="icon">💊</div>
                    <h3>Total Médicaments</h3>
                    <div class="value">${dashboard.totalMedicaments || 0}</div>
                    <div class="change">✓ Actifs</div>
                </div>
                <div class="stat-card">
                    <div class="icon">👥</div>
                    <h3>Total Clients</h3>
                    <div class="value">${dashboard.totalClients || 0}</div>
                    <div class="change">✓ Enregistrés</div>
                </div>
                <div class="stat-card">
                    <div class="icon">💰</div>
                    <h3>Chiffre d'Affaires</h3>
                    <div class="value">${chiffreAffaires.toFixed(2)}</div>
                    <div class="change">MAD</div>
                </div>
                <div class="stat-card">
                    <div class="icon">📊</div>
                    <h3>Total Ventes</h3>
                    <div class="value">${dashboard.totalVentes || 0}</div>
                    <div class="change">✓ Transactions</div>
                </div>
                <div class="stat-card">
                    <div class="icon">⚠️</div>
                    <h3>Stock Faible</h3>
                    <div class="value" style="color: var(--warning-amber);">${dashboard.medicamentsStockFaibleCount || dashboard.medicamentsStockFaible || 0}</div>
                    <div class="change" style="color: var(--warning-amber);">Attention requise</div>
                </div>
                <div class="stat-card">
                    <div class="icon">⏰</div>
                    <h3>Expire Bientôt</h3>
                    <div class="value" style="color: var(--warning-amber);">${dashboard.stocksExpireBientot || 0}</div>
                    <div class="change" style="color: var(--warning-amber);">Dans 30 jours</div>
                </div>
                <div class="stat-card">
                    <div class="icon">❌</div>
                    <h3>Stocks Expirés</h3>
                    <div class="value" style="color: var(--danger-red);">${dashboard.stocksExpires || 0}</div>
                    <div class="change" style="color: var(--danger-red);">Action immédiate</div>
                </div>
                <div class="stat-card">
                    <div class="icon">📋</div>
                    <h3>Commandes en Attente</h3>
                    <div class="value" style="color: var(--info-blue);">${dashboard.commandesEnAttente || 0}</div>
                    <div class="change" style="color: var(--info-blue);">À traiter</div>
                </div>
            </div>
            
            <div class="charts-container">
                <div class="chart-card">
                    <h3>📈 Ventes Mensuelles</h3>
                    <canvas id="ventesChart" style="max-height: 300px;"></canvas>
                </div>
                <div class="chart-card">
                    <h3>🏆 Médicaments les Plus Vendus</h3>
                    <canvas id="medicamentsChart" style="max-height: 300px;"></canvas>
                </div>
            </div>
            
            <div class="table-container" style="margin-top: 30px;">
                <h3 style="color: var(--secondary-green); margin-bottom: 20px; font-size: 1.3rem;">📊 Top 5 Médicaments les Plus Vendus</h3>
                <table>
                    <thead>
                        <tr>
                            <th>Médicament</th>
                            <th>Quantité Vendue</th>
                            <th>Chiffre d'Affaires</th>
                        </tr>
                    </thead>
                    <tbody>
                        ${medicamentsPopulaires.length > 0 ? 
                            medicamentsPopulaires.map(m => `
                                <tr>
                                    <td><strong>${m.nom || 'N/A'}</strong></td>
                                    <td><span class="badge badge-success">${m.quantiteVendue || 0}</span></td>
                                    <td><strong class="text-green">${(m.chiffreAffaires || 0).toFixed(2)} MAD</strong></td>
                                </tr>
                            `).join('') : 
                            '<tr><td colspan="3" class="empty-state">Aucune donnée disponible</td></tr>'
                        }
                    </tbody>
                </table>
            </div>
            
            <div class="table-container" style="margin-top: 30px;">
                <h3 style="color: var(--secondary-green); margin-bottom: 20px; font-size: 1.3rem;">📅 Détail Ventes Mensuelles</h3>
                <table>
                    <thead>
                        <tr>
                            <th>Mois</th>
                            <th>Nombre de Ventes</th>
                            <th>Montant Total</th>
                        </tr>
                    </thead>
                    <tbody>
                        ${ventesMensuelles.length > 0 ? 
                            ventesMensuelles.map(v => `
                                <tr>
                                    <td><strong>${v.mois || 'N/A'}</strong></td>
                                    <td>${v.nombreVentes || 0}</td>
                                    <td><strong class="text-green">${(v.montant || 0).toFixed(2)} MAD</strong></td>
                                </tr>
                            `).join('') : 
                            '<tr><td colspan="3" class="empty-state">Aucune donnée disponible</td></tr>'
                        }
                    </tbody>
                </table>
            </div>
        `;
        
        // Créer les graphiques Chart.js
        createCharts(dashboard);
    } catch (error) {
        container.innerHTML = `<div class="error">Erreur: ${error.message}</div>`;
    }
}

// Recherche de médicaments
let allMedicaments = [];

async function loadMedicaments(searchTerm = '') {
    const container = document.getElementById('medicaments-list');
    container.innerHTML = '<div class="loading">Chargement...</div>';
    
    try {
        const url = searchTerm ? `${API_BASE_URL}/medicaments?search=${encodeURIComponent(searchTerm)}` : `${API_BASE_URL}/medicaments`;
        const response = await fetch(url);
        const medicaments = await response.json();
        allMedicaments = medicaments;
        
        if (medicaments.length === 0) {
            container.innerHTML = '<div class="error">Aucun médicament trouvé</div>';
            return;
        }
        
        container.innerHTML = medicaments.map(m => {
            const id = m.id || m.Id;
            const nom = m.nom || m.Nom;
            const codeBarre = m.codeBarre || m.CodeBarre;
            const dosage = m.dosage || m.Dosage;
            const forme = m.forme || m.Forme;
            const prixVente = m.prixVente || m.PrixVente;
            const stockDisponible = m.stockDisponible || m.StockDisponible || 0;
            const stockMinimum = m.stockMinimum || m.StockMinimum || 0;
            const categorieNom = m.categorieNom || m.CategorieNom || 'Non catégorisé';
            
            return `
            <div class="card">
                <h3>${nom} <span style="font-size: 0.8em; color: #6b7280;">(ID: ${id})</span></h3>
                <p><strong>Code-barres:</strong> ${codeBarre}</p>
                <p><strong>Dosage:</strong> ${dosage || 'N/A'}</p>
                <p><strong>Forme:</strong> ${forme}</p>
                <p><strong>Prix:</strong> ${prixVente.toFixed(2)} MAD</p>
                <p><strong>Stock disponible:</strong> ${stockDisponible}</p>
                <p><strong>Stock minimum:</strong> ${stockMinimum}</p>
                <p><strong>Catégorie:</strong> ${categorieNom}</p>
                <span class="badge ${stockDisponible <= stockMinimum ? 'badge-danger' : 'badge-success'}">
                    ${stockDisponible <= stockMinimum ? 'Stock faible' : 'En stock'}
                </span>
            </div>
        `;
        }).join('');
    } catch (error) {
        container.innerHTML = `<div class="error">Erreur: ${error.message}</div>`;
    }
}

// Recherche
function searchMedicaments() {
    const searchTerm = document.getElementById('search-medicaments').value;
    loadMedicaments(searchTerm);
}

function searchClients() {
    const searchTerm = document.getElementById('search-clients').value;
    loadClients(searchTerm);
}

// Charger les clients
let allClients = [];

async function loadClients(searchTerm = '') {
    const container = document.getElementById('clients-list');
    container.innerHTML = '<div class="loading">Chargement...</div>';
    
    try {
        const url = searchTerm ? `${API_BASE_URL}/clients?search=${encodeURIComponent(searchTerm)}` : `${API_BASE_URL}/clients`;
        const response = await fetch(url);
        const clients = await response.json();
        allClients = clients;
        
        if (clients.length === 0) {
            container.innerHTML = '<div class="error">Aucun client trouvé</div>';
            return;
        }
        
        container.innerHTML = clients.map(c => `
            <div class="card">
                <h3>${c.nomComplet}</h3>
                <p><strong>Téléphone:</strong> ${c.telephone}</p>
                <p><strong>Email:</strong> ${c.email || 'N/A'}</p>
                <p><strong>Adresse:</strong> ${c.adresse || 'N/A'}</p>
            </div>
        `).join('');
    } catch (error) {
        container.innerHTML = `<div class="error">Erreur: ${error.message}</div>`;
    }
}

// Charger les ventes
async function loadVentes() {
    const container = document.getElementById('ventes-list');
    container.innerHTML = '<div class="loading">Chargement...</div>';
    
    try {
        const response = await fetch(`${API_BASE_URL}/ventes`);
        const ventes = await response.json();
        
        if (ventes.length === 0) {
            container.innerHTML = '<div class="error">Aucune vente trouvée</div>';
            return;
        }
        
        container.innerHTML = `
            <table>
                <thead>
                    <tr>
                        <th>N° Facture</th>
                        <th>Date</th>
                        <th>Client</th>
                        <th>Montant Total</th>
                        <th>Détails</th>
                    </tr>
                </thead>
                <tbody>
                    ${ventes.map(v => `
                        <tr>
                            <td>${v.numeroFacture}</td>
                            <td>${new Date(v.dateVente).toLocaleDateString('fr-FR')}</td>
                            <td>${v.clientNom || 'Client anonyme'}</td>
                            <td>${v.montantTotal.toFixed(2)} MAD</td>
                            <td>${v.details.length} article(s)</td>
                        </tr>
                    `).join('')}
                </tbody>
            </table>
        `;
    } catch (error) {
        container.innerHTML = `<div class="error">Erreur: ${error.message}</div>`;
    }
}

// Charger les stocks
async function loadStocks() {
    const container = document.getElementById('stocks-list');
    container.innerHTML = '<div class="loading">Chargement...</div>';
    
    try {
        const response = await fetch(`${API_BASE_URL}/stocks`);
        const stocks = await response.json();
        
        if (stocks.length === 0) {
            container.innerHTML = '<div class="error">Aucun stock trouvé</div>';
            return;
        }
        
        container.innerHTML = `
            <table>
                <thead>
                    <tr>
                        <th>Médicament</th>
                        <th>Quantité</th>
                        <th>N° Lot</th>
                        <th>Date Entrée</th>
                        <th>Date Expiration</th>
                        <th>Statut</th>
                    </tr>
                </thead>
                <tbody>
                    ${stocks.map(s => `
                        <tr>
                            <td>${s.medicamentNom}</td>
                            <td>${s.quantite}</td>
                            <td>${s.numeroLot}</td>
                            <td>${new Date(s.dateEntree).toLocaleDateString('fr-FR')}</td>
                            <td>${new Date(s.dateExpiration).toLocaleDateString('fr-FR')}</td>
                            <td>
                                <span class="badge ${s.estExpire ? 'badge-danger' : s.expireBientot ? 'badge-warning' : 'badge-success'}">
                                    ${s.estExpire ? 'Expiré' : s.expireBientot ? 'Expire bientôt' : 'Valide'}
                                </span>
                            </td>
                        </tr>
                    `).join('')}
                </tbody>
            </table>
        `;
    } catch (error) {
        container.innerHTML = `<div class="error">Erreur: ${error.message}</div>`;
    }
}

// Charger les commandes
async function loadCommandes() {
    const container = document.getElementById('commandes-list');
    container.innerHTML = '<div class="loading">Chargement...</div>';
    
    try {
        const response = await fetch(`${API_BASE_URL}/commandes`);
        const commandes = await response.json();
        
        if (commandes.length === 0) {
            container.innerHTML = '<div class="error">Aucune commande trouvée</div>';
            return;
        }
        
        container.innerHTML = `
            <table>
                <thead>
                    <tr>
                        <th>Date</th>
                        <th>Fournisseur</th>
                        <th>Statut</th>
                        <th>Montant Total</th>
                    </tr>
                </thead>
                <tbody>
                    ${commandes.map(c => `
                        <tr>
                            <td>${new Date(c.dateCommande).toLocaleDateString('fr-FR')}</td>
                            <td>${c.fournisseurNom}</td>
                            <td><span class="badge ${c.statut === 'Reçue' ? 'badge-success' : c.statut === 'Annulée' ? 'badge-danger' : 'badge-warning'}">${c.statut}</span></td>
                            <td>${c.montantTotal.toFixed(2)} MAD</td>
                        </tr>
                    `).join('')}
                </tbody>
            </table>
        `;
    } catch (error) {
        container.innerHTML = `<div class="error">Erreur: ${error.message}</div>`;
    }
}

// Charger les fournisseurs
async function loadFournisseurs() {
    const container = document.getElementById('fournisseurs-list');
    container.innerHTML = '<div class="loading">Chargement...</div>';
    
    try {
        const response = await fetch(`${API_BASE_URL}/fournisseurs`);
        const fournisseurs = await response.json();
        
        if (fournisseurs.length === 0) {
            container.innerHTML = '<div class="error">Aucun fournisseur trouvé</div>';
            return;
        }
        
        container.innerHTML = fournisseurs.map(f => `
            <div class="card">
                <h3>${f.nom}</h3>
                <p><strong>Téléphone:</strong> ${f.telephone}</p>
                <p><strong>Email:</strong> ${f.email || 'N/A'}</p>
                <p><strong>Adresse:</strong> ${f.adresse || 'N/A'}</p>
                <span class="badge ${f.estActif ? 'badge-success' : 'badge-danger'}">
                    ${f.estActif ? 'Actif' : 'Inactif'}
                </span>
            </div>
        `).join('');
    } catch (error) {
        container.innerHTML = `<div class="error">Erreur: ${error.message}</div>`;
    }
}

// Charger les prescriptions
async function loadPrescriptions() {
    const container = document.getElementById('prescriptions-list');
    container.innerHTML = '<div class="loading">Chargement...</div>';
    
    try {
        const response = await fetch(`${API_BASE_URL}/prescriptions`);
        const prescriptions = await response.json();
        
        if (prescriptions.length === 0) {
            container.innerHTML = '<div class="error">Aucune prescription trouvée</div>';
            return;
        }
        
        container.innerHTML = `
            <table>
                <thead>
                    <tr>
                        <th>N° Prescription</th>
                        <th>Date</th>
                        <th>Client</th>
                        <th>Médecin</th>
                        <th>Statut</th>
                    </tr>
                </thead>
                <tbody>
                    ${prescriptions.map(p => `
                        <tr>
                            <td>${p.numeroPrescription}</td>
                            <td>${new Date(p.datePrescription).toLocaleDateString('fr-FR')}</td>
                            <td>${p.clientNom}</td>
                            <td>${p.nomMedecin}</td>
                            <td><span class="badge ${p.estUtilisee ? 'badge-success' : 'badge-warning'}">${p.estUtilisee ? 'Utilisée' : 'Non utilisée'}</span></td>
                        </tr>
                    `).join('')}
                </tbody>
            </table>
        `;
    } catch (error) {
        container.innerHTML = `<div class="error">Erreur: ${error.message}</div>`;
    }
}

// Fonctions pour afficher les modals
async function showAddMedicamentForm() {
    console.log('showAddMedicamentForm appelée');
    const modal = document.getElementById('modal');
    const modalBody = document.getElementById('modal-body');
    
    if (!modal || !modalBody) {
        console.error('Modal ou modalBody introuvable');
        alert('Erreur: Modal introuvable. Veuillez recharger la page.');
        return;
    }
    
    // Charger les catégories depuis les médicaments existants
    let categoriesOptions = '<option value="1">Général (1)</option>';
    try {
        const response = await fetch(`${API_BASE_URL}/medicaments`, {
            headers: getAuthHeaders()
        });
        if (response.ok) {
            const medicaments = await response.json();
            const categories = [...new Set(medicaments.map(m => ({
                id: m.categorieId || m.CategorieId || 1,
                nom: m.categorieNom || m.CategorieNom || 'Général'
            })))];
            categoriesOptions = categories.map(c => 
                `<option value="${c.id}">${c.nom} (${c.id})</option>`
            ).join('') + '<option value="1">Général (1)</option>';
        }
    } catch (error) {
        console.error('Erreur chargement catégories:', error);
    }
    
    modalBody.innerHTML = `
        <h2 style="color: var(--secondary-green); margin-bottom: 20px;">➕ Ajouter un Médicament</h2>
        <form id="add-medicament-form" onsubmit="submitMedicament(event)">
            <div class="form-group">
                <label>Nom du médicament *</label>
                <input type="text" id="med-nom" class="form-control" required placeholder="ex: Paracétamol">
            </div>
            <div class="form-group">
                <label>Code-barres *</label>
                <input type="text" id="med-codebarre" class="form-control" required placeholder="ex: 1234567890123">
            </div>
            <div class="form-group">
                <label>Dosage</label>
                <input type="text" id="med-dosage" class="form-control" placeholder="ex: 500mg">
            </div>
            <div class="form-group">
                <label>Forme *</label>
                <select id="med-forme" class="form-control" required>
                    <option value="">Sélectionner...</option>
                    <option value="Comprimé">Comprimé</option>
                    <option value="Gélule">Gélule</option>
                    <option value="Sirop">Sirop</option>
                    <option value="Injection">Injection</option>
                    <option value="Crème">Crème</option>
                    <option value="Pommade">Pommade</option>
                    <option value="Gouttes">Gouttes</option>
                    <option value="Spray">Spray</option>
                </select>
            </div>
            <div class="form-group">
                <label>Prix d'achat (MAD) *</label>
                <input type="number" id="med-prix-achat" class="form-control" step="0.01" min="0" required placeholder="0.00">
            </div>
            <div class="form-group">
                <label>Prix de vente (MAD) *</label>
                <input type="number" id="med-prix-vente" class="form-control" step="0.01" min="0" required placeholder="0.00">
            </div>
            <div class="form-group">
                <label>Stock minimum *</label>
                <input type="number" id="med-stock-min" class="form-control" min="0" value="10" required>
            </div>
            <div class="form-group">
                <label>Catégorie *</label>
                <select id="med-categorie" class="form-control" required>
                    ${categoriesOptions}
                </select>
                <small class="text-muted">Si la catégorie n'existe pas, utilisez l'ID 1 (Général)</small>
            </div>
            <div class="form-group">
                <label>
                    <input type="checkbox" id="med-prescription"> Requiert une prescription
                </label>
            </div>
            <div class="form-actions">
                <button type="button" class="btn btn-secondary" onclick="closeModal()">Annuler</button>
                <button type="submit" class="btn btn-primary">Créer</button>
            </div>
        </form>
    `;
    modal.style.display = 'block';
}

function showAddClientForm() {
    const modal = document.getElementById('modal');
    const modalBody = document.getElementById('modal-body');
    
    modalBody.innerHTML = `
        <h2 style="color: var(--secondary-green); margin-bottom: 20px;">➕ Ajouter un Client</h2>
        <form id="add-client-form" onsubmit="submitClient(event)">
            <div class="form-group">
                <label>Nom *</label>
                <input type="text" id="client-nom" class="form-control" required>
            </div>
            <div class="form-group">
                <label>Prénom *</label>
                <input type="text" id="client-prenom" class="form-control" required>
            </div>
            <div class="form-group">
                <label>Téléphone *</label>
                <input type="tel" id="client-telephone" class="form-control" required>
            </div>
            <div class="form-group">
                <label>Email</label>
                <input type="email" id="client-email" class="form-control">
            </div>
            <div class="form-group">
                <label>Date de naissance</label>
                <input type="date" id="client-date-naissance" class="form-control">
            </div>
            <div class="form-group">
                <label>Adresse</label>
                <textarea id="client-adresse" class="form-control" rows="3"></textarea>
            </div>
            <div class="form-actions">
                <button type="button" class="btn btn-secondary" onclick="closeModal()">Annuler</button>
                <button type="submit" class="btn btn-primary">Créer</button>
            </div>
        </form>
    `;
    modal.style.display = 'block';
}

async function showAddStockForm() {
    const modal = document.getElementById('modal');
    const modalBody = document.getElementById('modal-body');
    
    // Charger les médicaments pour le select
    let medicamentsOptions = '<option value="">Chargement...</option>';
    try {
        const response = await fetch(`${API_BASE_URL}/medicaments`, {
            headers: getAuthHeaders()
        });
        if (response.ok) {
            const medicaments = await response.json();
            medicamentsOptions = '<option value="">Sélectionner un médicament...</option>' +
                medicaments.map(m => `<option value="${m.id || m.Id}">${m.nom || m.Nom} - ${m.codeBarre || m.CodeBarre}</option>`).join('');
        }
    } catch (error) {
        console.error('Erreur chargement médicaments:', error);
        medicamentsOptions = '<option value="">Erreur de chargement</option>';
    }
    
    modalBody.innerHTML = `
        <h2 style="color: var(--secondary-green); margin-bottom: 20px;">➕ Ajouter un Stock</h2>
        <form id="add-stock-form" onsubmit="submitStock(event)">
            <div class="form-group">
                <label>Médicament *</label>
                <select id="stock-medicament-id" class="form-control" required>
                    ${medicamentsOptions}
                </select>
            </div>
            <div class="form-group">
                <label>Quantité *</label>
                <input type="number" id="stock-quantite" class="form-control" min="1" required>
            </div>
            <div class="form-group">
                <label>Numéro de lot</label>
                <input type="text" id="stock-numero-lot" class="form-control" placeholder="ex: LOT-001">
            </div>
            <div class="form-group">
                <label>Date d'expiration *</label>
                <input type="date" id="stock-date-expiration" class="form-control" required>
            </div>
            <div class="form-group">
                <label>Notes</label>
                <textarea id="stock-notes" class="form-control" rows="2" placeholder="Notes optionnelles"></textarea>
            </div>
            <div class="form-actions">
                <button type="button" class="btn btn-secondary" onclick="closeModal()">Annuler</button>
                <button type="submit" class="btn btn-primary">Créer</button>
            </div>
        </form>
    `;
    modal.style.display = 'block';
}

function showAddVenteForm() {
    alert('Pour créer une vente, utilisez le module Caissier dans le menu de navigation.');
}

function showAddCommandeForm() {
    alert('Fonctionnalité à implémenter - Utilisez Swagger pour créer des commandes');
}

function showAddFournisseurForm() {
    alert('Fonctionnalité à implémenter - Utilisez Swagger pour ajouter des fournisseurs');
}

function showAddPrescriptionForm() {
    alert('Fonctionnalité à implémenter - Utilisez Swagger pour créer des prescriptions');
}

// Fonctions de soumission
async function submitMedicament(event) {
    event.preventDefault();
    
    const data = {
        nom: document.getElementById('med-nom').value.trim(),
        codeBarre: document.getElementById('med-codebarre').value.trim(),
        dosage: document.getElementById('med-dosage').value.trim() || '',
        forme: document.getElementById('med-forme').value,
        prixAchat: parseFloat(document.getElementById('med-prix-achat').value),
        prixVente: parseFloat(document.getElementById('med-prix-vente').value),
        stockMinimum: parseInt(document.getElementById('med-stock-min').value),
        categorieId: parseInt(document.getElementById('med-categorie').value),
        requiertPrescription: document.getElementById('med-prescription').checked
    };
    
    // Validation
    if (data.prixAchat < 0 || data.prixVente < 0) {
        alert('❌ Les prix ne peuvent pas être négatifs');
        return;
    }
    
    if (data.prixVente < data.prixAchat) {
        if (!confirm('⚠️ Le prix de vente est inférieur au prix d\'achat. Continuer ?')) {
            return;
        }
    }
    
    // Vérifier le token avant d'envoyer la requête
    const token = localStorage.getItem('token');
    if (!token) {
        alert('❌ Vous n\'êtes pas connecté. Veuillez vous reconnecter.');
        window.location.href = '/login.html';
        return;
    }
    
    console.log('Tentative de création de médicament...');
    console.log('URL:', `${API_BASE_URL}/medicaments`);
    console.log('Headers:', getAuthHeaders());
    console.log('Données:', data);
    
    try {
        const response = await fetch(`${API_BASE_URL}/medicaments`, {
            method: 'POST',
            headers: getAuthHeaders(),
            body: JSON.stringify(data)
        });
        
        console.log('Réponse reçue:', response.status, response.statusText);
        
        if (!response.ok) {
            let errorMessage = `Erreur HTTP ${response.status}: ${response.statusText}`;
            try {
                const errorData = await response.json();
                console.error('Erreur détaillée:', errorData);
                errorMessage = errorData.error || errorData.message || 
                              (errorData.errors ? JSON.stringify(errorData.errors) : errorMessage);
            } catch (e) {
                const text = await response.text();
                console.error('Erreur texte:', text);
                if (text) errorMessage = text;
            }
            throw new Error(errorMessage);
        }
        
        const medicament = await response.json();
        console.log('Médicament créé avec succès:', medicament);
        alert('✅ Médicament créé avec succès!');
        closeModal();
        loadMedicaments(); // Recharger la liste
    } catch (error) {
        let errorMsg = error.message;
        if (error.message === 'Failed to fetch' || error.name === 'TypeError') {
            errorMsg = 'Impossible de contacter le serveur.\n\n' +
                      'Vérifications:\n' +
                      '1. Le backend est-il en cours d\'exécution ?\n' +
                      '2. Vérifiez la console du navigateur (F12) pour plus de détails\n' +
                      '3. Vérifiez les logs du backend\n' +
                      '4. Essayez de rafraîchir la page (F5)';
        }
        alert('❌ Erreur: ' + errorMsg);
        console.error('Erreur création médicament:', error);
        console.error('Type d\'erreur:', error.name);
        console.error('Message:', error.message);
        console.error('Stack:', error.stack);
        console.error('Données envoyées:', data);
    }
}

async function submitClient(event) {
    event.preventDefault();
    
    const dateNaissance = document.getElementById('client-date-naissance').value;
    const email = document.getElementById('client-email').value.trim();
    
    const data = {
        nom: document.getElementById('client-nom').value.trim(),
        prenom: document.getElementById('client-prenom').value.trim(),
        telephone: document.getElementById('client-telephone').value.trim(),
        email: email || null,
        dateNaissance: dateNaissance ? new Date(dateNaissance).toISOString() : null,
        adresse: document.getElementById('client-adresse').value.trim() || null
    };
    
    // Validation
    if (email && !email.includes('@')) {
        alert('❌ Email invalide');
        return;
    }
    
    try {
        const response = await fetch(`${API_BASE_URL}/clients`, {
            method: 'POST',
            headers: getAuthHeaders(),
            body: JSON.stringify(data)
        });
        
        if (!response.ok) {
            const errorData = await response.json();
            const errorMessage = errorData.error || errorData.message || 'Erreur lors de la création';
            throw new Error(errorMessage);
        }
        
        const client = await response.json();
        alert('✅ Client créé avec succès!');
        closeModal();
        loadClients(); // Recharger la liste
    } catch (error) {
        alert('❌ Erreur: ' + error.message);
        console.error('Erreur création client:', error);
    }
}

async function submitStock(event) {
    event.preventDefault();
    
    const dateExpiration = document.getElementById('stock-date-expiration').value;
    
    const data = {
        medicamentId: parseInt(document.getElementById('stock-medicament-id').value),
        quantite: parseInt(document.getElementById('stock-quantite').value),
        numeroLot: document.getElementById('stock-numero-lot').value || '',
        dateExpiration: dateExpiration ? new Date(dateExpiration).toISOString() : null,
        notes: document.getElementById('stock-notes').value || null
    };
    
    try {
        const response = await fetch(`${API_BASE_URL}/stocks`, {
            method: 'POST',
            headers: getAuthHeaders(),
            body: JSON.stringify(data)
        });
        
        if (!response.ok) {
            const error = await response.json();
            throw new Error(error.error || error.message || 'Erreur lors de la création');
        }
        
        const stock = await response.json();
        alert('✅ Stock ajouté avec succès!');
        closeModal();
        loadStocks(); // Recharger la liste
    } catch (error) {
        alert('❌ Erreur: ' + error.message);
    }
}

function closeModal() {
    document.getElementById('modal').style.display = 'none';
}

// Fermer la modal en cliquant en dehors
window.onclick = function(event) {
    const modal = document.getElementById('modal');
    if (event.target === modal) {
        closeModal();
    }
}

// Créer les graphiques Chart.js
function createCharts(dashboard) {
    const ventesMensuelles = dashboard.ventesMensuelles || [];
    const medicamentsPopulaires = dashboard.medicamentsPopulaires || [];
    
    // Graphique des ventes mensuelles (Bar Chart) - Thème Vert
    const ventesCtx = document.getElementById('ventesChart');
    if (ventesCtx) {
        new Chart(ventesCtx, {
            type: 'bar',
            data: {
                labels: ventesMensuelles.map(v => v.mois || 'N/A'),
                datasets: [{
                    label: 'Montant (MAD)',
                    data: ventesMensuelles.map(v => v.montant || 0),
                    backgroundColor: 'rgba(16, 185, 129, 0.8)',
                    borderColor: 'rgba(5, 150, 105, 1)',
                    borderWidth: 2,
                    borderRadius: 8
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: true,
                plugins: {
                    legend: {
                        display: false
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true,
                        ticks: {
                            callback: function(value) {
                                return value.toFixed(0) + ' MAD';
                            }
                        }
                    }
                }
            }
        });
    }
    
    // Graphique des médicaments les plus vendus (Horizontal Bar Chart) - Thème Vert
    const medicamentsCtx = document.getElementById('medicamentsChart');
    if (medicamentsCtx && medicamentsPopulaires.length > 0) {
        new Chart(medicamentsCtx, {
            type: 'bar',
            data: {
                labels: medicamentsPopulaires.map(m => m.nom || 'N/A'),
                datasets: [{
                    label: 'Quantité Vendue',
                    data: medicamentsPopulaires.map(m => m.quantiteVendue || 0),
                    backgroundColor: 'rgba(16, 185, 129, 0.8)',
                    borderColor: 'rgba(5, 150, 105, 1)',
                    borderWidth: 2,
                    borderRadius: 8
                }]
            },
            options: {
                indexAxis: 'y',
                responsive: true,
                maintainAspectRatio: true,
                plugins: {
                    legend: {
                        display: false
                    }
                },
                scales: {
                    x: {
                        beginAtZero: true
                    }
                }
            }
        });
    }
}

// Charger les alertes
async function loadAlertes() {
    const container = document.getElementById('alertes-list');
    container.innerHTML = '<div class="loading">Chargement des alertes...</div>';
    
    try {
        // Récupérer les données pour les alertes
        const [medicamentsResponse, stocksResponse, commandesResponse] = await Promise.all([
            fetch(`${API_BASE_URL}/medicaments/stock-faible`),
            fetch(`${API_BASE_URL}/stocks/expire-bientot`),
            fetch(`${API_BASE_URL}/commandes`)
        ]);
        
        const medicamentsStockFaible = await medicamentsResponse.json();
        const stocksExpireBientot = await stocksResponse.json();
        const commandes = await commandesResponse.json();
        const commandesEnAttente = commandes.filter(c => c.statut === 'En attente');
        
        const alertes = [];
        
        // Alertes stock faible
        if (medicamentsStockFaible.length > 0) {
            alertes.push({
                type: 'warning',
                titre: '⚠️ Stock Faible',
                message: `${medicamentsStockFaible.length} médicament(s) en stock faible`,
                details: medicamentsStockFaible.map(m => m.nom).join(', ')
            });
        }
        
        // Alertes expiration
        if (stocksExpireBientot.length > 0) {
            alertes.push({
                type: 'danger',
                titre: '⏰ Expiration Proche',
                message: `${stocksExpireBientot.length} stock(s) expire(nt) bientôt`,
                details: stocksExpireBientot.map(s => `${s.medicamentNom} - ${new Date(s.dateExpiration).toLocaleDateString('fr-FR')}`).join(', ')
            });
        }
        
        // Alertes commandes
        if (commandesEnAttente.length > 0) {
            alertes.push({
                type: 'info',
                titre: '📋 Commandes en Attente',
                message: `${commandesEnAttente.length} commande(s) en attente de réception`,
                details: commandesEnAttente.map(c => `Commande #${c.id} - ${c.fournisseurNom}`).join(', ')
            });
        }
        
        // Mettre à jour le badge
        const badge = document.getElementById('alertes-badge');
        if (badge) {
            if (alertes.length > 0) {
                badge.textContent = alertes.length;
                badge.style.display = 'inline-block';
            } else {
                badge.style.display = 'none';
            }
        }
        
        if (alertes.length === 0) {
            container.innerHTML = '<div class="success">✅ Aucune alerte pour le moment</div>';
            return;
        }
        
        container.innerHTML = alertes.map(alerte => `
            <div class="alerte-card ${alerte.type}">
                <h3>${alerte.titre}</h3>
                <p><strong>${alerte.message}</strong></p>
                <p class="alerte-details">${alerte.details}</p>
            </div>
        `).join('');
    } catch (error) {
        container.innerHTML = `<div class="error">Erreur: ${error.message}</div>`;
    }
}

function refreshAlertes() {
    loadAlertes();
}

// Actualiser les alertes toutes les 30 secondes
setInterval(() => {
    if (document.getElementById('alertes').classList.contains('active')) {
        loadAlertes();
    } else {
        // Vérifier le badge même si l'onglet n'est pas actif
        fetch(`${API_BASE_URL}/medicaments/stock-faible`)
            .then(r => r.json())
            .then(data => {
                const badge = document.getElementById('alertes-badge');
                if (badge && data.length > 0) {
                    badge.textContent = data.length;
                    badge.style.display = 'inline-block';
                }
            });
    }
}, 30000);

// Fonction pour masquer/afficher les éléments selon le rôle
function applyRoleBasedUI() {
    const user = JSON.parse(localStorage.getItem('user') || '{}');
    const role = user.roleName || '';

    // Pour le Caissier, masquer certains onglets et boutons
    if (role === 'Caissier') {
        // Masquer les onglets non accessibles
        const tabsToHide = ['medicaments', 'commandes', 'fournisseurs'];
        tabsToHide.forEach(tabName => {
            const tabBtn = document.querySelector(`[data-tab="${tabName}"]`);
            if (tabBtn) tabBtn.style.display = 'none';
        });

        // Masquer les boutons d'ajout dans Stocks
        const stocksAddBtn = document.querySelector('#stocks .btn-primary');
        if (stocksAddBtn && stocksAddBtn.textContent.includes('Ajouter Stock')) {
            stocksAddBtn.style.display = 'none';
        }
    }
}

// Dashboard simplifié pour le Caissier
async function loadDashboardCaissier() {
    const container = document.getElementById('dashboard-content');
    container.innerHTML = '<div class="loading">Chargement...</div>';
    
    try {
        const response = await fetch(`${API_BASE_URL}/dashboard/caissier`, {
            headers: getAuthHeaders()
        });
        
        if (!response.ok) {
            const errorText = await response.text();
            console.error('Erreur API:', response.status, errorText);
            throw new Error(`Erreur ${response.status}: ${errorText || 'Erreur lors du chargement du dashboard'}`);
        }
        
        const dashboard = await response.json();
        console.log('Dashboard data:', dashboard);
        
        container.innerHTML = `
            <div class="dashboard-grid">
                <div class="dashboard-card">
                    <h3>💰 Ventes Aujourd'hui</h3>
                    <p class="stat-value">${(dashboard.totalVentesAujourdhui || 0).toFixed(0)}</p>
                </div>
                <div class="dashboard-card">
                    <h3>💵 Chiffre d'Affaires</h3>
                    <p class="stat-value">${(dashboard.chiffreAffairesAujourdhui || 0).toFixed(2)} MAD</p>
                </div>
            </div>
            
            <div style="margin-top: 30px;">
                <h3>📊 Médicaments les Plus Vendus</h3>
                <div class="table-container">
                    <table>
                        <thead>
                            <tr>
                                <th>Médicament</th>
                                <th>Quantité Vendue</th>
                            </tr>
                        </thead>
                        <tbody>
                            ${(dashboard.medicamentsPlusVendus || []).slice(0, 5).map(m => `
                                <tr>
                                    <td>${m.nom || 'N/A'}</td>
                                    <td>${m.quantiteVendue || 0}</td>
                                </tr>
                            `).join('') || '<tr><td colspan="2">Aucune donnée disponible</td></tr>'}
                        </tbody>
                    </table>
                </div>
            </div>
            
            <div style="margin-top: 30px;">
                <h3>⚠️ Alertes Stock Faible</h3>
                <div class="table-container">
                    <table>
                        <thead>
                            <tr>
                                <th>Médicament</th>
                                <th>Stock Disponible</th>
                                <th>Stock Minimum</th>
                            </tr>
                        </thead>
                        <tbody>
                            ${(dashboard.alertesStockFaible || []).slice(0, 5).map(m => `
                                <tr>
                                    <td>${m.nom || 'N/A'}</td>
                                    <td><span class="badge badge-danger">${m.stockDisponible || 0}</span></td>
                                    <td>${m.stockMinimum || 0}</td>
                                </tr>
                            `).join('') || '<tr><td colspan="3">Aucune alerte</td></tr>'}
                        </tbody>
                    </table>
                </div>
            </div>
        `;
    } catch (error) {
        console.error('Erreur complète:', error);
        container.innerHTML = `
            <div class="error">
                <h3>❌ Erreur lors du chargement du dashboard</h3>
                <p><strong>Message:</strong> ${error.message}</p>
                <p style="margin-top: 10px; font-size: 0.9em; color: #666;">
                    <strong>Solution:</strong> Assurez-vous que la migration a été appliquée à la base de données.<br>
                    Exécutez: <code>dotnet ef database update</code>
                </p>
            </div>
        `;
    }
}

// Charger les données au démarrage
document.addEventListener('DOMContentLoaded', () => {
    // Vérifier l'authentification
    if (!checkAuth()) {
        return;
    }

    // Afficher les informations de l'utilisateur
    const user = JSON.parse(localStorage.getItem('user') || '{}');
    if (user.username) {
        const header = document.querySelector('header h1');
        if (header) {
            header.innerHTML = `
                <span>🏥 Système de Gestion de Pharmacie</span>
                <div id="user-info">
                    <i class="bi bi-person-circle"></i> ${user.username} 
                    <span class="badge badge-success" style="margin-left: 8px; padding: 4px 10px;">${user.roleName}</span>
                    <a href="#" onclick="logout(); return false;" style="color: white; text-decoration: none; margin-left: 15px; padding: 5px 10px; border-radius: 5px; background: rgba(255,255,255,0.2); transition: all 0.3s;" onmouseover="this.style.background='rgba(255,255,255,0.3)'" onmouseout="this.style.background='rgba(255,255,255,0.2)'">
                        <i class="bi bi-box-arrow-right"></i> Déconnexion
                    </a>
                </div>
            `;
        }
    }

    // Appliquer les restrictions d'interface selon le rôle
    applyRoleBasedUI();

    // Charger le dashboard selon le rôle
    const role = user.roleName || '';
    if (role === 'Caissier') {
        // Rediriger vers le module caissier dédié
        window.location.href = '/caissier/dashboard.html';
    } else {
        loadDashboard();
    }

    // Charger les alertes en arrière-plan pour le badge
    setTimeout(() => {
        fetch(`${API_BASE_URL}/medicaments/stock-faible`)
            .then(r => r.json())
            .then(data => {
                const badge = document.getElementById('alertes-badge');
                if (badge && data.length > 0) {
                    badge.textContent = data.length;
                    badge.style.display = 'inline-block';
                }
            });
    }, 1000);
});

// Fonction de déconnexion
function logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    localStorage.removeItem('refreshToken');
    window.location.href = '/login.html';
}

