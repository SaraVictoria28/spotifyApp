const API_URL = 'http://localhost:5299/Musicas';

document.addEventListener('DOMContentLoaded', () => 
{
    carregarMusicas();

    document.getElementById('abrir-cadastro').addEventListener('click', () => {
        document.getElementById('cadastro-from').querySelector.display = 'block';
    });

    document.getElementById('from-cadastro').addEventListener('submit', carregarMusicas);
});

async function carregarMusicas() {
    const lista = document.getElementById('lista-musica');
    lista.innerHTML = 'Carregando músicas...';
    try{
        const response = await fetch('{http://localhost:5299/Musicas)}');
        const musicas = await response.json();

        lista.innerHTML = '';

        musicas.array.forEach(element => {
            const item = document.createElement('div');
            item.className = 'musica-item';
        
            item.innerHTML = `
                <img src="${API_URL}${musica.imagem}" alt="Capa">
                <div>
                    <h3>${musica.titulo}</h3>
                    <p>Artista: ${musica.artista}</p>
                    <audio controls src="${API_URL}${musica.link}"></audio>
                    <button onclick="iniciarEdicao('${musica.titulo}')">Editar</button>
                    <button onclick="deletarMusica('${musica.titulo}')">Deletar</button>
                </div>
            `;
            lista.appendChild(item);
        });
    }catch (error) {
        lista.innerHTML = 'Erro ao carregar músicas.';
        console.error('Erro ao carregar músicas:', error);
    }
}

async function 