const URL_API = 'https://localhost:7081/api/v1/Documento';
async function enviarDocumento() {
    const codigoCliente = document.getElementById("codigoCliente").value;
    const inputArquivo = document.getElementById("arquivo");
    const arquivo = inputArquivo.files[0];

    if (!codigoCliente || !arquivo) {
        alert("Informe o codigo do cliente e seleciona um arquivo");
        return;
    }
    const dadosArquivos = new FormData();
    dadosArquivos.append("arquivo", arquivo);

    const response = await fetch(`${URL_API}/upload/${codigoCliente}`, {
        method: "POST",
        body: dadosArquivos
    });
    if (response.ok) {
        alert("Documento enviado com sucesso");
        document.getElementById("codigoCliente").value = "";
        document.getElementById("arquivo").value = "";
    } else {
        alert("Falha ao enviar o arquivo");
    }
}
async function listarDocumentos() {

    const codigoCliente = document.getElementById("codigoCliente").value;

    if (!codigoCliente) {
        alert("Informe o código do cliente");
        return;
    }

    const response = await fetch(`${URL_API}/listar/${codigoCliente}`);

    if (response.ok) {

        const documentos = await response.json();
        const corpoTabela = document.getElementById("corpoTabela");
        corpoTabela.innerHTML = "";

        documentos.forEach(documento => {

            const linha = document.createElement("tr");

            linha.innerHTML = `
                <td>${documento.id}</td>
                <td>${documento.name}</td>
                <td>${documento.extensao}</td>
                <td>
                <button onclick="baixarDocumento(${documento.id})">
                Download
                </button>
                <button onclick="excluirDocumento(${documento.id})">
                Excluir
                </button>
                </td>

            `;

            corpoTabela.appendChild(linha);
        });

    } else {

        alert("Nenhum documento foi encontrado para este cliente");

    }
}
async function baixarDocumento(id) {

    const response = await fetch(`${URL_API}/download/${id}`);

    if (response.ok) {

        const arquivo = await response.blob();

        const url = window.URL.createObjectURL(arquivo);

        const link = document.createElement("a");

        link.href = url;
        link.download = `documento-${id}`;

        link.click();

        window.URL.revokeObjectURL(url);

    } else {

        alert("Não foi possível baixar o documento");

    }
}
async function excluirDocumento(id) {

    const confirmar = confirm("Tem certeza que deseja excluir este documento?");

    if (!confirmar) {
        return;
    }

    const response = await fetch(`${URL_API}/excluir/${id}`, {
        method: "DELETE"
    });

    if (response.ok) {
        alert("Documento excluído com sucesso");

        listarDocumentos();
    } else {
        alert("Não foi possível excluir o documento");
    }
}