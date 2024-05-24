import axios from "axios";

const cdnUrl = process.env.REACT_APP_CDN_URL;

const client = axios.create({
  baseURL: cdnUrl,
});

// POST /cdn/image/upload
const uploadImage = async (payload: any): Promise<string> => {
  try {
    const res = await client.post("upload", payload);
    return res.status === 200 ? res.data.image : "";
  } catch (err) {
    console.log(err);
    return "";
  }
};

const imageService = { uploadImage };
export default imageService;
