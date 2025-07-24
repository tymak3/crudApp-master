// Configuration
const API_BASE_URL = 'https://localhost:7237'; // Updated to match your API port

// DOM Elements
const loadStatsBtn = document.getElementById('loadStatsBtn');
const loading = document.getElementById('loading');
const error = document.getElementById('error');
const playerStats = document.getElementById('playerStats');

// Event Listeners
loadStatsBtn.addEventListener('click', loadPlayerStats);

// Main function to load player statistics
async function loadPlayerStats() {
    showLoading();
    
    try {
        // Call your new players endpoint to get real data
        const response = await fetch(`${API_BASE_URL}/api/automations/players`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
            }
        });

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        const players = await response.json();
        console.log('API Response:', players);
        
        // Display the real player data
        displayPlayerData(players);
        
    } catch (err) {
        console.error('Error loading player stats:', err);
        showError();
    } finally {
        hideLoading();
    }
}

// Display player data in cards
function displayPlayerData(players) {
    playerStats.innerHTML = '';
    
    players.forEach((player, index) => {
        const playerCard = createPlayerCard(player, index + 1);
        playerStats.appendChild(playerCard);
    });
}

// Create a player card element
function createPlayerCard(player, rank) {
    const col = document.createElement('div');
    col.className = 'col-md-6 col-lg-4';
    
    col.innerHTML = `
        <div class="player-card">
            <div class="d-flex justify-content-between align-items-center">
                <div>
                    <h5 class="mb-1">${player.playerName}</h5>
                    <small class="text-muted">Rank #${rank}</small>
                </div>
                <div class="distance-badge">
                    ${player.averageDrivingDistance} yards
                </div>
            </div>
        </div>
    `;
    
    return col;
}

// UI Helper Functions
function showLoading() {
    loading.classList.remove('d-none');
    error.classList.add('d-none');
    playerStats.innerHTML = '';
    loadStatsBtn.disabled = true;
}

function hideLoading() {
    loading.classList.add('d-none');
    loadStatsBtn.disabled = false;
}

function showError() {
    error.classList.remove('d-none');
    loading.classList.add('d-none');
}

// Optional: Load data automatically when page loads
// window.addEventListener('load', loadPlayerStats);
