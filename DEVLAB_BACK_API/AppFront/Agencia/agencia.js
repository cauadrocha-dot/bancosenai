const URL_API = 'https://localhost:7081/api/v1/Agencia';
let modoEdicao = false;

// Carregar dados ao abrir a página
document.addEventListener("DOMContentLoaded", listarAgencias);

async function listarAgencias() {
    const response = await fetch(URL_API);
    const agencias = await response.json();
    const corpo = document.getElementById('corpoTabela');
    corpo.innerHTML = '';

    agencias.forEach(a => {
        corpo.innerHTML += `
            <tr>
                <td>${a.numeroAgencia}</td>
                <td>${a.cidade}</td>
                <td>${a.siglaEstado}</td>
                <td>
                    <button class="btn-editar" onclick="prepararEdicao(${a.numeroAgencia}, '${a.cidade}', '${a.siglaEstado}')">Editar</button>
                    <button class="btn-excluir" onclick="excluirAgencia(${a.numeroAgencia})">Excluir</button>
                </td>
            </tr>`;
    });
}

async function salvar() {
    const num = document.getElementById('numAgencia').value;
    const cid = document.getElementById('cidade').value;
    const est = document.getElementById('siglaEstado').value;

    // Validação: só salva se todos os campos estiverem preenchidos
    if (!num || !cid || !est) {
        alert("Por favor, preencha todos os campos antes de salvar.");
        return;
    }

    const agencia = { numeroAgencia: parseInt(num), cidade: cid, siglaEstado: est };
    
    // Define se usa POST (Criar) ou PUT (Alterar) conforme a regra REST [3]
    const metodo = modoEdicao ? 'PUT' : 'POST';
    const urlFinal = modoEdicao ? `${URL_API}/${num}` : URL_API;

    const response = await fetch(urlFinal, {
        method: metodo,
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(agencia)
    });

    if (response.ok) {
        alert(modoEdicao ? "Agência atualizada!" : "Agência cadastrada com sucesso!");
        limparCampos();
        listarAgencias();
    } else {
        const erro = await response.json();
        alert("Erro: " + (erro.message || "Falha na operação"));
    }
}

function prepararEdicao(num, cid, est) {
    document.getElementById('numAgencia').value = num;
    document.getElementById('numAgencia').disabled = true; // Impede alterar o código único
    document.getElementById('cidade').value = cid;
    document.getElementById('siglaEstado').value = est;
    modoEdicao = true;
}

async function excluirAgencia(num) {
    // Caixa de diálogo de confirmação conforme solicitado
    if (confirm(`Deseja realmente excluir a agência ${num}?`)) {
        const response = await fetch(`${URL_API}/${num}`, { method: 'DELETE' });
        if (response.ok) {
            listarAgencias(); // Remove da linha e da memória RAM visualmente
        }
    }
}

function limparCampos() {
    document.getElementById('numAgencia').value = '';
    document.getElementById('numAgencia').disabled = false;
    document.getElementById('cidade').value = '';
    document.getElementById('siglaEstado').value = '';
    modoEdicao = false;
}