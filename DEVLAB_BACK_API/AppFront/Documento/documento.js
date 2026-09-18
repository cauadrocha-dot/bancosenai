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
                <td>${documento.Extensao}</td>
                <td>Ações</td>
            `;

            corpoTabela.appendChild(linha);
        });

    } else {

        alert("Nenhum documento foi encontrado para este cliente");

    }
}