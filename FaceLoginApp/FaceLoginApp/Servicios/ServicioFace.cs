using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Plugin.Media.Abstractions;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using FaceLoginApp.Helpers;
using FaceLoginApp.Modelos;

namespace FaceLoginApp.Servicios
{
    public static class ServicioFace
    {
        public static async Task<Emocion> ObtenerEmocion(MediaFile foto)
        {
            Emocion emocion = null;

            try
            {
                if (foto != null)
                {
                    var clienteFace = new FaceServiceClient(Constantes.FaceApiKey, Constantes.FaceApiURL);
                    var atributosFace = new FaceAttributeType[] { FaceAttributeType.Emotion };

                    using (var stream = foto.GetStream())
                    {
                        Face[] rostros = await clienteFace.DetectAsync(stream, false, false, atributosFace);

                        if (rostros.Any())
                        {
                            var analisisEmocion = rostros.FirstOrDefault().FaceAttributes.Emotion.ToRankedList().FirstOrDefault();
                            emocion = new Emocion()
                            {
                                Nombre = analisisEmocion.Key,
                                Score = analisisEmocion.Value,
                                Foto = foto.Path
                            };
                        }

                        foto.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return emocion;
        }

        public static async Task<bool> CrearGrupoEmpleados()
        {
            try
            {
                var clienteFace = new FaceServiceClient(Constantes.FaceApiKey, Constantes.FaceApiURL);
                await clienteFace.CreatePersonGroupAsync(Constantes.FaceGroupID, Constantes.FaceGroupName, Constantes.FaceGroupDescription);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async static Task<Guid> RegistrarPersonaEnGrupo(string nombre)
        {
            try
            {
                var clienteFace = new FaceServiceClient(Constantes.FaceApiKey, Constantes.FaceApiURL);
                var resultado = await clienteFace.CreatePersonAsync(Constantes.FaceGroupID, nombre);
                return resultado.PersonId;
            }
            catch (Exception ex)
            {
                return new Guid();
            }
        }

        public async static Task<Guid> RegistrarRostro(Guid personID, Stream stream)
        {
            try
            {
                var clienteFace = new FaceServiceClient(Constantes.FaceApiKey, Constantes.FaceApiURL);
                var resultado = await clienteFace.AddPersonFaceAsync(Constantes.FaceGroupID, personID, stream);
                return resultado.PersistedFaceId;
            }
            catch (Exception ex)
            {
                return new Guid();
            }
        }

        public async static Task<bool> EntrenarGrupoEmpleados()
        {
            try
            {
                var clienteFace = new FaceServiceClient(Constantes.FaceApiKey, Constantes.FaceApiURL);
                await clienteFace.TrainPersonGroupAsync(Constantes.FaceGroupID);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async static Task<Guid> DetectarRostro(Stream stream)
        {
            try
            {
                var clienteFace = new FaceServiceClient(Constantes.FaceApiKey, Constantes.FaceApiURL);
                var faces = await clienteFace.DetectAsync(stream);

                if (faces.Count() > 0)
                    return faces[0].FaceId;

                return new Guid();
            }
            catch (Exception ex)
            {
                return new Guid();
            }
        }

        public async static Task<Guid> IdentificarEmpleado(Guid faceID)
        {
            try
            {
                var faceIDs = new Guid[] { faceID };
                var clienteFace = new FaceServiceClient(Constantes.FaceApiKey, Constantes.FaceApiURL);
                var resultado = await clienteFace.IdentifyAsync(Constantes.FaceGroupID, faceIDs);

                if (resultado.Count() > 0)
                    if (resultado[0].Candidates.Count() > 0)
                        return resultado[0].Candidates[0].PersonId;

                return new Guid();
            }
            catch (Exception ex)
            {
                return new Guid();
            }
        }
    }
}
